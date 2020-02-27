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


        [HttpPost("refresh/{id}")]
        public ActionResult Refresh(int id)
        {
            CourseTemplate template = _context.CourseTemplates.Where(t => t.CourseTemplateId == id).First();
            dockerService.RunContainer(template.DockerRefreshImage, null, data =>
            {
                Console.WriteLine("GOT A REPLY");
                Console.WriteLine(data);
            });
            return Ok("Ok");
        }
    }
}
