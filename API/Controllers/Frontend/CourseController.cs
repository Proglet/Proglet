using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Frontend.Course;
using API.ORM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proglet.Core.Data;

namespace API.Controllers
{
    /// <summary>
    /// Shows different courses
    /// </summary>
    [Route("[controller]")]
    [Controller]
    public class CourseController : Controller
    {
        private readonly DataContext context;

        /// <summary>
        /// DI constructor
        /// </summary>
        /// <param name="context"></param>
        public CourseController(DataContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Page showing info about a specific course
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult ViewCourse(int id)
        {
            return View(new CourseModel()
            {
                Course = context.Courses
                    .Where(c => c.CourseTemplateId == id)
                    .Include(c => c.CourseTemplate)
                    .Include(c => c.Submissions)
                        .ThenInclude(s => s.User)
                    .Include(c => c.Submissions)
                        .ThenInclude(s => s.TestResults)
                    .First(),
                Exercises = context.Exercises.Where(e => e.CourseTemplateId == id).ToList(),
                Enrolled = context.Users.Where(u => u.CourseRegistrations.Any(cr => cr.User == u && cr.Active)).ToList()
            });
        }
    }
}
