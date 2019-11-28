using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Proglet.Core.Data
{
    public class Point : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long PointId { get; set; }

        public Exercise Exercise { get; set; } // Of andersom?

        public string Name { get; set; }


    }
}
