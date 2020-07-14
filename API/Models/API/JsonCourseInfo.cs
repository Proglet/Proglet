using System;
using System.Collections.Generic;

namespace API.Controllers
{
    class JsonCourseInfo
    {
        public string subject { get; set; }
        public string name { get; set; }
        public string hash { get; set; }
        public int size { get; set; }
        public List<String> points { get; set; }
    }
}
