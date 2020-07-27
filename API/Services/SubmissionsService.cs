using API.Models.Multipart;
using Microsoft.EntityFrameworkCore;
using API.ORM;
using Microsoft.Extensions.Hosting;
using Proglet.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Services
{
    public interface ISubmissionsService
    {
        Submission Queue(int userId, ExerciseSubmission submission, string ip);
    }


    public class SubmissionsService : BackgroundService, ISubmissionsService
    {
        private EventWaitHandle waitHandle;
        private IDockerService dockerService;

        public SubmissionsService(IDockerService dockerService)
        {
            this.dockerService = dockerService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            await Task.Delay(3000); //omg yuck

            while (!cancellationToken.IsCancellationRequested)
            {
                waitHandle.WaitOne(TimeSpan.FromSeconds(60));

                using (var context = new DataContext(null))
                {
                    context.Submissions
                        .Where(s => !s.Processed)
                        .ToList()
                        .ForEach(s =>
                        {
                            /*dockerService.RunContainer("", new Dictionary<string, string>(), new Action<byte[]>((data) =>
                            {
                                Console.WriteLine("Done testing exercise!");

                            }));*/
                            s.Processed = true;
                            Console.WriteLine($"Processing submission {s.SubmissionId}");
                        });
                    await context.SaveChangesAsync();
                }
            }
        }


        public Submission Queue(int userId, ExerciseSubmission submission, string ip)
        {
            using (var context = new DataContext(null))
            {
                CourseRegistration cr = context.CourseRegistrations.Where(cr => cr.CourseId == submission.CourseId && cr.UserId == userId).Include(cr => cr.User).FirstOrDefault();
                if (cr == null || !cr.Active)
                    throw new Exception("You are not registered for this course");

                string exerciseSubject = submission.ExerciseName.Substring(0, submission.ExerciseName.IndexOf("/"));
                string exerciseName = submission.ExerciseName.Substring(submission.ExerciseName.IndexOf("/")+1);

                Exercise exercise = context.Exercises
                    .Where(e => e.Name == exerciseName && e.Subject == exerciseSubject)
                    .FirstOrDefault();

                if (exercise == null)
                    throw new Exception("Exercise not found");

                Submission s = new Submission();
                s.User = cr.User;
                s.Exercise = exercise;
                s.Processed = false;
                s.SubmissionIp = ip;
                s.SubmissionTime = DateTime.Now;
                s.SubmissionZip = new BinaryReader(submission.Data.OpenReadStream()).ReadBytes((int)submission.Data.Length);

                context.Submissions.Add(s);
                context.SaveChanges();
                waitHandle.Set();
                return s;
            }

        }
    }
}
