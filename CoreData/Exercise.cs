using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Proglet.Core.Data
{
#nullable enable
    public class Exercise
    {

        public int ExerciseId { get; set; }

        public int CourseTemplateId { get; set; }
        
        [ForeignKey("CourseTemplateId")]
        public Course CourseTemplate { get; set; }

        [Column(TypeName = "VARCHAR(64)")]
        [Required]
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
        public List<Point> Points { get; set; }

        public DockerTestImage DockerTestImage { get; set; }
    }

#nullable restore
}
