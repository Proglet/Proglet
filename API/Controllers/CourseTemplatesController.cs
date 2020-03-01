using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreDataORM;
using Proglet.Core.Data.Internal;
using API.Services;
using System.IO.Compression;
using System.IO;
using System.Text.Json;
using Proglet.Core.Data;

namespace API.Controllers
{
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
        }

        // GET: api/CourseTemplates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseTemplate>>> GetCourseTemplates()
        {
            return await _context.CourseTemplates.ToListAsync();
        }

        // GET: api/CourseTemplates/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseTemplate>> GetCourseTemplate(long id)
        {
            var courseTemplate = await _context.CourseTemplates.FindAsync(id);

            if (courseTemplate == null)
            {
                return NotFound();
            }

            return courseTemplate;
        }

        // PUT: api/CourseTemplates/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourseTemplate(long id, CourseTemplate courseTemplate)
        {
            if (id != courseTemplate.CourseTemplateId)
            {
                return BadRequest();
            }

            _context.Entry(courseTemplate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseTemplateExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CourseTemplates
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<CourseTemplate>> PostCourseTemplate(CourseTemplate courseTemplate)
        {
            _context.CourseTemplates.Add(courseTemplate);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourseTemplate", new { id = courseTemplate.CourseTemplateId }, courseTemplate);
        }

        // DELETE: api/CourseTemplates/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CourseTemplate>> DeleteCourseTemplate(long id)
        {
            var courseTemplate = await _context.CourseTemplates.FindAsync(id);
            if (courseTemplate == null)
            {
                return NotFound();
            }

            _context.CourseTemplates.Remove(courseTemplate);
            await _context.SaveChangesAsync();

            return courseTemplate;
        }

        private bool CourseTemplateExists(long id)
        {
            return _context.CourseTemplates.Any(e => e.CourseTemplateId == id);
        }


        class JsonCourseInfo
        {
            public string subject { get; set; }
            public string name { get; set; }
            public string hash { get; set; }
            public int size { get; set; }
            public List<String> points { get; set; }
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
                using (var exercisesStream = archive.GetEntry("exercises.json").Open())
                {
                    //TODO: remove/disable exercises that are in database but not in json file
                    List<JsonCourseInfo> exercises = JsonSerializer.Deserialize<List<JsonCourseInfo>>(new StreamReader(exercisesStream).ReadToEnd());

                    foreach (var exercise in exercises)
                    {
                        Console.WriteLine($"Checking exercise {exercise.subject}/{exercise.name}");
                        var lastExercise = dbcontext.Exercises
                            .Where(e =>
                                e.Course.CourseId == id &&
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
                            CourseId = id,
                            Name = exercise.name,
                            Subject = exercise.name,
                            Version = version,
                            Checksum = exercise.hash,
                            Size = exercise.size,
                            CreatedAt = DateTime.Now,
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

                Console.WriteLine(data);
            }));
            return Ok("Ok");
        }
    }
}
