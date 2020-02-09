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

        public long CourseId { get; set; }

        [Column(TypeName = "bit")]
        public bool Enabled { get; set; }

        public CourseTemplate CourseTemplate { get; set; }

        public DateTime? HidderAfter { get; set; }

        public ICollection<CourseRegistration> Users { get; set; }
    }
}
