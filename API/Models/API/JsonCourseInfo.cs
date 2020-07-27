using System;
using System.Collections.Generic;

namespace API.Controllers
{
    class JsonCourseInfo
    {
        public class Test
        {
            public string name { get; set; }
            public string className { get; set; }
            public string point { get; set; }
        }
        public string subject { get; set; }
        public string name { get; set; }
        public string hash { get; set; }
        public int size { get; set; }
        public List<String> points { get; set; }
        public List<Test> tests { get; set; }
    }
}
