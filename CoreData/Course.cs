﻿using Proglet.Core.Data.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Proglet.Core.Data
{
    public class Course
    {


        public int CourseId { get; set; }

        public int CourseTemplateId { get; set; }
        [ForeignKey("CourseTemplateId")]
        public CourseTemplate CourseTemplate { get; set; }

        [Column(TypeName = "bit")]
        public bool Enabled { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string Curriculum { get; set; }

        [Column(TypeName = "DATE")]
        public DateTime HidderAfter { get; set; }

        public ICollection<CourseRegistration> Users { get; set; }

        public ICollection<Submission> Submissions { get; set; }
    }
}
