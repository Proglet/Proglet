using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Proglet.Core.Data
{
    public class CourseRegistration
    {
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        [Column(TypeName = "bit")]
        public bool Active { get; set; }
    }
}
