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

        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string MaterialUrl { get; set; }

        [Column(TypeName = "bit")]
        public bool Enabled { get; set; }


        public CourseTemplate CourseTemplate { get; set; }


        public DateTime? HidderAfter { get; set; }

        public User CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime RefeshedAt { get; set; }


    }
}
