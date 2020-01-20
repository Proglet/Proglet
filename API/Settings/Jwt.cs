using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Settings
{
    public class Jwt
    {
        public string SecretPreLogin { get; set; }
        public string Secret { get; set; }
    }
}
