using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proglet.Core.Data;
using API.Models.Multipart;
using API.Services;
using System.Globalization;
using API.ORM;
using System.IO;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionsService submissions;
        private readonly DataContext context;

        public SubmissionsController(ISubmissionsService submissions, DataContext context)
        {
            this.submissions = submissions;
            this.context = context;
        }


        [HttpPost("Submit")]
        public IActionResult Submit([FromForm] ExerciseSubmission submission)
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == "client_id").Value, CultureInfo.InvariantCulture);

            Submission newSubmission = submissions.Queue(
                userId: userId, 
                submission: submission, 
                ip: Request.HttpContext.Connection.RemoteIpAddress.ToString());
            Console.WriteLine(submission.CourseId);
            Console.WriteLine(submission.ExerciseName);
            Console.WriteLine(submission.Data.FileName);
            return Ok();
        }


        //TODO: add authentication
        [HttpGet("download/{id}")]
        public IActionResult Download(int id)
        {
            var submission = context.Submissions.FirstOrDefault(s => s.SubmissionId == id);
            if (submission == null)
                return Problem("Submission not found");

            var contentType = "Application/octet-stream";
            var fileName = $"Submission_{id}.zip";

            return File(submission.SubmissionZip, contentType, fileName);
        }
    }
}
