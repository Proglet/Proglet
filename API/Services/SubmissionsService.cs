using API.Models.Multipart;
using CoreDataORM;
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
        Submission Queue(int userId, ExerciseSubmission submission, string ip, DataContext context);
    }


    public class SubmissionsService : BackgroundService, ISubmissionsService
    {
        private EventWaitHandle waitHandle;
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

            while (!cancellationToken.IsCancellationRequested)
            {
                waitHandle.WaitOne(TimeSpan.FromSeconds(1));

                //TODO: get a context here... :(
            }
        }


        public Submission Queue(int userId, ExerciseSubmission submission, string ip, DataContext context)
        {
            CourseRegistration cr = context.CourseRegistrations.Where(cr => cr.CourseId == submission.CourseId && cr.UserId == userId).FirstOrDefault();
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
