﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Proglet.Core.Data
{
    public class Point : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int PointId { get; set; }

        public Exercise Exercise { get; set; } 

        public List<Test> Tests { get; set; }

        public string Name { get; set; }



    }
}
