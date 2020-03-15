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

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        private readonly DataContext _context;

        public ExercisesController(DataContext context)
        {
            _context = context;
        }


        [HttpPost("Submit")]
        public IActionResult Submit([FromForm] ExerciseSubmission submission)
        {
            Console.WriteLine(submission.CourseId);
            Console.WriteLine(submission.ExerciseName);
            Console.WriteLine(submission.Data.FileName);
            return Ok();
        }


    }
}
