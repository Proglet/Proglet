using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreDataORM;
using Proglet.Core.Data;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly DataContext _context;

        public CoursesController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Object>>> GetCourses()
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == "client_id").Value);

            return await _context.Courses
                .Include(e => e.CourseTemplate)
                .Where(e => e.Enabled && e.HidderAfter > DateTime.Now)
                .Select(e => new
                {
                    id = e.CourseId,
                    Name = e.CourseTemplate.Name,
                    Title = e.CourseTemplate.Title,
                    Description = e.CourseTemplate.Description,
                    Registered = e.Users.Any(u => u.UserId == userId && u.Active)
                }).ToListAsync();
        }


        [HttpPost("Enroll/{id}")]
        public ActionResult Enroll(int id)
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == "client_id").Value);
            CourseRegistration cr = _context.CourseRegistrations.Where(cr => cr.CourseId == id && cr.UserId == userId).FirstOrDefault();

            if (cr == null)
            {
                cr = new CourseRegistration()
                {
                    UserId = userId,
                    CourseId = id,
                    Active = true
                };
                _context.CourseRegistrations.Add(cr);
            }
            else
                cr.Active = true;
            _context.SaveChanges();
            return Ok("enrolled");
        }
        [HttpPost("Unregister/{id}")]
        public ActionResult Unregister(int id)
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == "client_id").Value);
            CourseRegistration cr = _context.CourseRegistrations.Where(cr => cr.CourseId == id && cr.UserId == userId).FirstOrDefault();
            if (cr != null)
                cr.Active = false;
            _context.SaveChanges();
            return Ok("unregistered");
        }

    }
}