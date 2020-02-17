using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DockerSlaveManager
{
    public class Config
    {
        public string ConnectionString { get; set; }
        public string SharedMount { get; set; }
        public string SharedMountLocal { get; set; }
    }
}
