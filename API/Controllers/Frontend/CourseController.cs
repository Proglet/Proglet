using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Frontend.Course;
using CoreDataORM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proglet.Core.Data;

namespace API.Controllers
{
    [Route("[controller]")]
    [Controller]
    public class CourseController : Controller
    {
        private readonly DataContext context;

        public CourseController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ViewCourse(int id)
        {
            return View(new CourseModel()
            {
                Course = context.Courses.Where(c => c.CourseTemplateId == id).Include(c => c.CourseTemplate).First(),
                Exercises = context.Exercises.Where(e => e.CourseTemplateId == id).ToList(),
                Enrolled = context.Users.Where(u => u.CourseRegistrations.Any(cr => cr.User == u && cr.Active)).ToList()
            });
        }
    }
}
