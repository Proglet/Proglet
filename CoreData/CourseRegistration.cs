using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Proglet.Core.Data
{
    public class CourseRegistration : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int UserId { get; set; }
        public User User { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Column(TypeName = "bit")]
        public bool Active { get; set; }
    }
}
