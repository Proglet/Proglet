using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Proglet.Core.Data.Internal
{
    public class CourseTemplate : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [Column(TypeName = "varchar(64)")]
        public string Name { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string Title { get; set; }

        [Column(TypeName = "text")]
        public string Description { get; set; }

        public long CourseTemplateId { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string GitUrl { get; set; }
        //TODO: gitauth

        public DateTime CreatedOn { get; set; }

        public User CreatedBy { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string MaterialUrl { get; set; }
    }
}
