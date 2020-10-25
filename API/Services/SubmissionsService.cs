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
using API.Settings;
using Microsoft.Extensions.Options;
using System.IO.Compression;
using System.Xml;

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
        private Docker dockerConfig;

        public SubmissionsService(IDockerService dockerService, IOptions<Docker> config)
        {
            this.dockerService = dockerService;
            this.dockerConfig = config.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            await Task.Delay(3000).ConfigureAwait(true); //omg yuck

            while (!cancellationToken.IsCancellationRequested)
            {
                waitHandle.WaitOne(TimeSpan.FromSeconds(10));

                using (var context = new DataContext(null))
                {
                    context.Submissions
                        .Where(s => s.Status == Submission.SubmissionStatus.Unprocessed)
                        .Include(s => s.Exercise)
                            .ThenInclude(e => e.DockerTestImage)
                        .Include(s => s.Exercise)
                            .ThenInclude(e => e.CourseTemplate)
                        .ToList()
                        .ForEach(s =>
                        {
                            List<string> mounts = new List<string>()
                            {
                                "/tests:" + dockerConfig.CallbackUrl + "/api/coursetemplates/DownloadMainProject/" + s.Exercise.CourseTemplateId,
                                "/usercode:" + dockerConfig.CallbackUrl + "/api/submissions/download/" + s.SubmissionId,
                            };

                            dockerService.RunContainer(
                                image: s.Exercise.DockerTestImage.ImageName, 
                                mounts: mounts, 
                                outDir: "/out",
                                environment: new Dictionary<string, string>() { { "TEST", $":{s.Exercise.Subject}-{s.Exercise.Name}:test" } },
                                callback: new Action<byte[]>(async (data) =>
                            {
                                Console.WriteLine("Done testing exercise!");

                                using (ZipArchive zipArchive = new ZipArchive(new MemoryStream(data)))
                                {
                                    foreach (var entry in zipArchive.Entries)
                                    {
                                        if (!entry.FullName.EndsWith(".xml"))
                                            continue;
                                        using (var stream = entry.Open())
                                        using (var streamreader = new StreamReader(stream))
                                        {
                                            string xmlData = streamreader.ReadToEnd();
                                            XmlDocument document = new XmlDocument();
                                            document.LoadXml(xmlData);
                                            XmlNodeList cases = document.DocumentElement.GetElementsByTagName("testcase");
                                            using (var dbContext = new DataContext(null))
                                            {
                                                foreach (XmlNode @case in cases)
                                                {
                                                    var testName = @case.Attributes["name"].Value;
                                                    var className = @case.Attributes["classname"].Value;
                                                    double time = double.Parse(@case.Attributes["time"].Value);
                                                    bool Pass = true;
                                                    string StackTrace = "";
                                                    string message = "";

                                                    if(@case.ChildNodes.Count > 0)
                                                    {
                                                        string text = @case.FirstChild.InnerText;
                                                        string[] splitted = text.Split("\n", 2);
                                                        message = splitted[0];
                                                        StackTrace = splitted[1];
                                                    }


                                                    Console.WriteLine($"Ran test {className}.{testName} in {time}s");


                                                    var test = dbContext.Tests.FirstOrDefault(t => t.ClassName == className && t.Name == testName);
                                                    dbContext.TestResults.Add(new TestResult()
                                                    {
                                                        SubmissionId = s.SubmissionId,
                                                        Message = message,
                                                        Pass = Pass,
                                                        StackTrace = StackTrace,
                                                        Test = test
                                                    });
                                                }

                                                var ss = dbContext.Submissions.FirstOrDefault(Submission => Submission.SubmissionId == s.SubmissionId);
                                                ss.Status = Submission.SubmissionStatus.Processed;
                                                dbContext.Submissions.Update(ss);
                                                await dbContext.SaveChangesAsync();
                                            }
                                        }
                                    }


                                    

                                }
                            }));
                            s.Status = Submission.SubmissionStatus.Processing;
                            context.Submissions.Update(s);
                            Console.WriteLine($"Processing submission {s.SubmissionId}");
                        });
                    await context.SaveChangesAsync().ConfigureAwait(true);
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

                string exerciseSubject = submission.ExerciseName.Substring(0, submission.ExerciseName.IndexOf('/', StringComparison.InvariantCulture));
                string exerciseName = submission.ExerciseName.Substring(submission.ExerciseName.IndexOf('/', StringComparison.InvariantCulture) +1);

                Exercise exercise = context.Exercises
                    .Where(e => e.Name == exerciseName && e.Subject == exerciseSubject)
                    .FirstOrDefault();

                if (exercise == null)
                    throw new Exception("Exercise not found");

                using (var br = new BinaryReader(submission.Data.OpenReadStream()))
                {
                    Submission s = new Submission()
                    {
                        User = cr.User,
                        JobId = "",
                        CourseId = cr.CourseId,
                        Exercise = exercise,
                        Status = Submission.SubmissionStatus.Unprocessed,
                        SubmissionIp = ip,
                        SubmissionTime = DateTime.Now,
                        SubmissionZip = br.ReadBytes((int)submission.Data.Length)
                    };

                    context.Submissions.Add(s);
                    context.SaveChanges();

                    waitHandle.Set();
                    return s; //TOOO, returning in an using sounds like a bad idea
                }
            }

        }
    }
}
