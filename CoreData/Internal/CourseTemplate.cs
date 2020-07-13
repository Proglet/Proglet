using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Proglet.Core.Data.Internal
{
    public class CourseTemplate : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [Column(TypeName = "varchar(64)")]
        [Required]
        public string Name { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string Title { get; set; }

        [Column(TypeName = "text")]
        public string Description { get; set; }

        public int CourseTemplateId { get; set; }

        [Column(TypeName = "varchar(255)")]
        [Required]
        public string GitUrl { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string DockerRefreshImage { get; set; }

        public DateTime CreatedOn { get; set; }

        public User CreatedBy { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string MaterialUrl { get; set; }
    }
}
