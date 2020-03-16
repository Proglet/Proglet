using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreDataORM;
using Proglet.Core.Data;
using API.Models.Multipart;
using API.Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionsService submissions;

        public SubmissionsController(ISubmissionsService submissions)
        {
            this.submissions = submissions;
        }


        [HttpPost("Submit")]
        public IActionResult Submit([FromForm] ExerciseSubmission submission)
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == "client_id").Value);

            Submission newSubmission = submissions.Queue(
                userId: userId, 
                submission: submission, 
                ip: Request.HttpContext.Connection.RemoteIpAddress.ToString());
            Console.WriteLine(submission.CourseId);
            Console.WriteLine(submission.ExerciseName);
            Console.WriteLine(submission.Data.FileName);
            return Ok();
        }
    }
}
