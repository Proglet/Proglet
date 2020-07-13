using Proglet.Core.Data.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Proglet.Core.Data
{
    public class Course : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
    }
}
