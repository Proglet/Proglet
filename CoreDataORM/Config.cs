using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreDataORM
{
    public class Config
    {
        public string host { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string database { get; set; }

        public string configstring {
            get {
                return $"server={host};database={database};user={username};password={password}";
            }
        }
    }
}
