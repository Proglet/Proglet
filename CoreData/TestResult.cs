using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Proglet.Core.Data
{
    public class TestResult
    {

        public int TestResultId { get; set; }

        public Submission Submission { get; set; }

        public Test Test { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public bool Pass { get; set; }


    }
}
