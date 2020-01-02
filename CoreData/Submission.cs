using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Proglet.Core.Data
{
    public class Submission : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long SubmissionId { get; set; }

        public User User { get; set; }

        public Exercise Exercise { get; set; }

        /// <summary>
        /// Indicates if the submission is processed
        /// </summary>
        [Column(TypeName = "bit")]
        public bool Processed { get; set; }

        // some fields with results




        // Client info fields
        public string SubmissionIp { get; set; }

        public DateTime SubmissionTime { get; set; }


        [NotMapped]
        public byte[] SubmissionZip { get; set; }



    }
}
