using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proglet.Core.Data;
using Microsoft.AspNetCore.Authorization;
using System.IO.Compression;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using API.ORM;
using System.Globalization;

namespace API.Controllers
{
    /// <summary>
    /// Controller to manage courses. Can be used to list and enroll into courses
    /// </summary>
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

        /// <summary>
        /// GET: api/Courses
        /// </summary>
        /// <returns>Returns a list of all courses available to the current user</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Object>>> GetCourses()
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == "client_id").Value, CultureInfo.InvariantCulture);

            return await _context.Courses
                .Include(e => e.CourseTemplate)
                .Where(e => e.Enabled && e.HidderAfter > DateTime.Now)
                .Select(e => new
                {
                    id = e.CourseTemplateId,
                    e.CourseTemplate.Name,
                    e.CourseTemplate.Title,
                    e.CourseTemplate.Description,
                    e.Curriculum,
                    Registered = e.Users.Any(u => u.UserId == userId && u.Active)
                }).ToListAsync().ConfigureAwait(true);
        }


        [HttpPost("Enroll/{id}")]
        public ActionResult Enroll(int id)
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == "client_id").Value, CultureInfo.InvariantCulture);
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
            int userId = int.Parse(User.Claims.First(c => c.Type == "client_id").Value, CultureInfo.InvariantCulture);
            CourseRegistration cr = _context.CourseRegistrations.Where(cr => cr.CourseId == id && cr.UserId == userId).FirstOrDefault();
            if (cr != null)
                cr.Active = false;
            _context.SaveChanges();
            return Ok("unregistered");
        }

        /// <summary>
        /// Downloads the main project file
        /// uses the project.zip file in the template, updates the infofile and adds a field to the course-info.yaml to add the courseid so the proglet plugin knows where to submit back to
        /// </summary>
        /// <param name="id">The project ID</param>
        /// <returns></returns>
        [HttpGet("DownloadMainProject/{id}")]
        public ActionResult DownloadMainProject(int id)
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == "client_id").Value, CultureInfo.InvariantCulture);
            CourseRegistration cr = _context.CourseRegistrations.Where(cr => cr.CourseId == id && cr.UserId == userId).Include(cr => cr.Course).Include(cr => cr.Course.CourseTemplate).FirstOrDefault();
            if (cr == null)
                return Problem("Not enrolled in this course");

            int templateId = cr.Course.CourseTemplate.CourseTemplateId;
            string zipFileName = "data/templates/" + templateId + "/project.zip";

            //TODO: move this to some methods
            Dictionary<string, object> infoYaml;

            using (var fs = new FileStream(zipFileName, FileMode.Open))
            using(var zipFile = new ZipArchive(fs))
            { 
                ZipArchiveEntry info = zipFile.GetEntry("course-info.yaml");
                using (var stream = info.Open())
                using (var reader = new StreamReader(stream))
                {
                    var deserializer = new DeserializerBuilder()
                                   .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                   .Build();
                    infoYaml = deserializer.Deserialize<Dictionary<string,object>>(reader);
                }
            }
            infoYaml["title"] = cr.Course.CourseTemplate.Name + " - " + cr.Course.Curriculum;
            infoYaml["courseid"] = id;

            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var newInfo = serializer.Serialize(infoYaml);

            using (var newFile = new MemoryStream())
            {
                using (var fs = new FileStream(zipFileName, FileMode.Open))
                    fs.CopyTo(newFile);
                using (var zipFile = new ZipArchive(newFile, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry info = zipFile.GetEntry("course-info.yaml");
                    using (var stream = info.Open())
                    using (var writer = new StreamWriter(stream))
                        writer.Write(newInfo);
                }


                var contentType = "Application/octet-stream";
                var fileName = "Project.zip";
                return File(newFile.ToArray(), contentType, fileName);
            }
        }
    }
}