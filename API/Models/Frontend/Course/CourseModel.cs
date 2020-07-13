using Proglet.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Frontend.Course
{
    public class CourseModel
    {
        public Proglet.Core.Data.Course Course { get; set; }
        public List<Exercise> Exercises { get; set; }
        public List<User> Enrolled { get; set; }


    }
}
