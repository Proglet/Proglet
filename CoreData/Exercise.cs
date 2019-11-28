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

        public long ExerciseId { get; set; }

        public Course Course { get; set; }

        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime PublishDate { get; set; }

        [Column(TypeName = "bit")]
        public bool Hidden { get; set; }

        public Guid Checksum { get; set; }

        public DateTime SolutionVisableAfter { get; set; }

        [Column(TypeName = "bit")]
        public bool HasTests { get; set; }
        public string? RunTimeParameters { get; set; }

        [Column(TypeName = "bit")]
        public bool CodeReviewEnable { get; set; }


    }

#nullable restore
}
