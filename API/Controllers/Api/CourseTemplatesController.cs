using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proglet.Core.Data.Internal;
using API.Services;
using System.IO.Compression;
using System.IO;
using System.Text.Json;
using Proglet.Core.Data;
using API.ORM;

namespace API.Controllers
{
    /// <summary>
    /// Controller to manage course templates. Can be used to refresh a course template
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CourseTemplatesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IDockerService dockerService;

        public CourseTemplatesController(DataContext context, IDockerService dockerService)
        {
            _context = context;
            this.dockerService = dockerService;
            if (!Directory.Exists("data/templates"))
                Directory.CreateDirectory("data/templates");
        }

        [HttpPost("refresh/{id}")]
        public ActionResult Refresh(int id)
        {
            CourseTemplate template = _context.CourseTemplates.Where(t => t.CourseTemplateId == id).First();
            dockerService.RunContainer(template.DockerRefreshImage, null, new Action<byte[], DataContext>((data, dbcontext) =>
            { //in this callback, the _context is disposed already, so we use dbcontext
                Console.WriteLine("Course Template refresh got a reply...");
                //TODO: put this in background
                //TODO: move this to courses service?
                ZipArchive archive = new ZipArchive(new MemoryStream(data));
                foreach(var entry in archive.Entries)
                    Console.WriteLine(entry.FullName);

                if (Directory.Exists("data/templates/" + id))
                    Directory.Delete("data/templates/" + id, true);
                Directory.CreateDirectory("data/templates/" + id);
                using (var exercisesStream = archive.GetEntry("exercises.json").Open())
                {
                    //TODO: remove/disable exercises that are in database but not in json file
                    List<JsonCourseInfo> exercises = JsonSerializer.Deserialize<List<JsonCourseInfo>>(new StreamReader(exercisesStream).ReadToEnd());

                    foreach (var exercise in exercises)
                    {
                        Console.WriteLine($"Checking exercise {exercise.subject}/{exercise.name}");
                        var lastExercise = dbcontext.Exercises
                            .Where(e =>
                                e.CourseTemplate.CourseTemplateId == id &&
                                e.Name == exercise.name &&
                                e.Subject == exercise.subject)
                            .OrderBy(e => e.Version)
                            .LastOrDefault();
                        int version = 1;

                        if(lastExercise != null)
                        {
                            if (lastExercise.Checksum != exercise.hash || lastExercise.Size != exercise.size)
                            {
                                version = lastExercise.Version + 1;
                                Console.WriteLine($"Updating to version {version}");
                            }
                            else
                            {
                                Console.WriteLine("Skipping");
                                continue; // skip this exercise, doesn't have to be updated
                            }
                        }
                        var ex = new Exercise()
                        {
                            CourseTemplateId = id,
                            Name = exercise.name,
                            Subject = exercise.subject,
                            Version = version,
                            Checksum = exercise.hash,
                            Size = exercise.size,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            PublishDate = DateTime.Now,
                            SolutionVisableAfter = DateTime.Now
                        };
                        dbcontext.Exercises.Add(ex);
                        dbcontext.SaveChanges();

                        foreach(var point in exercise.points)
                        {
                            dbcontext.Points.Add(new Point()
                            {
                                Exercise = ex,
                                Name = point
                            });
                        }
                        dbcontext.SaveChanges();



                    }
                }
                //TODO: determine what zipfiles to copy, softcode in some kind of configfile?
                using (var zipStream = archive.GetEntry("ExampleCourse-Intellij-EduTools.zip").Open())
                using(var outStream = new FileStream("data/templates/" + id + "/project.zip", FileMode.Create))
                {
                    zipStream.CopyTo(outStream);
                }


                Console.WriteLine(data);
            }));
            return Ok("Ok");
        }
    }
}
