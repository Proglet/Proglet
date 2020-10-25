using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Proglet.Core.Data
{
    public class Submission
    {
        public enum SubmissionStatus
        {
            Unprocessed,
            Processing,
            Processed
        }

        public int SubmissionId { get; set; }

        public User User { get; set; }

        public Exercise Exercise { get; set; }


        [Column(TypeName = "enum(Unprocessed,Processing,Processed)")]
        public SubmissionStatus Status { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string JobId { get; set; }

        // some fields with results
        public List<TestResult> TestResults { get; set; }




        // Client info fields
        [Column(TypeName = "varchar(16)")]
        public string SubmissionIp { get; set; }

        public DateTime SubmissionTime { get; set; }


        //data
        [MaxLength(1024*1024*16), Column(TypeName = "LongBlob")] 
        public byte[] SubmissionZip { get; set; }



    }
}
