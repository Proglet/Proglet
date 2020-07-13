using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Multipart
{
    public class ExerciseSubmission
    {
        public int CourseId { get; set; }
        public string ExerciseName { get; set; }
        public IFormFile Data { get; set; }
    }
}
