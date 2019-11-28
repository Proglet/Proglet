using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Proglet.Core.Data.Internal
{
    public class CourseTemplate : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long CourseTemplateId { get; set; }
        public string GitUrl { get; set; }

        public DateTime CreatedOn { get; set; }

        public User CreatedBy { get; set; }
    }
}
