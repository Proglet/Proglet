using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Proglet.Core.Data
{
#nullable enable
    public class Exercise : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int ExerciseId { get; set; }

        public int CourseTemplateId { get; set; }
        
        [ForeignKey("CourseTemplateId")]
        public Course CourseTemplate { get; set; }

        [Column(TypeName = "VARCHAR(64)")]
        public string Name { get; set; }
        
        [Column(TypeName = "VARCHAR(64)")]
        public string Subject { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime PublishDate { get; set; }

        [Column(TypeName = "bit")]
        public bool Hidden { get; set; }

        //TODO: make this a binary value
        [Column(TypeName = "varchar(64)")]
        public string Checksum { get; set; }
        public int Size { get; set; }

        public int Version { get; set; }

        public DateTime SolutionVisableAfter { get; set; }

        [Column(TypeName = "bit")]
        public bool HasTests { get; set; }
        public string? RunTimeParameters { get; set; }

        [Column(TypeName = "bit")]
        public bool CodeReviewEnable { get; set; }


    }

#nullable restore
}
