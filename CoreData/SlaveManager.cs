using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Proglet.Core.Data
{
    public class SlaveManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string Url { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string Auth { get; set; }

        [Column(TypeName = "bit")]
        public bool Enabled { get; set; }

    }
}
