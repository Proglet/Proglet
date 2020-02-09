using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Settings
{
    public class Login : Dictionary<string, LoginInfo>
    {
        
    }

    public class LoginInfo
    {
        public string Type { get; set; }
        public OAuth OAuth { get; set; }
    }

    public class OAuth
    {
        public string RequestUrl { get; set; }
        public string LoginUrl { get; set; }
        public string TokenUrl { get; set; }
        public string InfoUrl { get; set; }
        public string MoreInfoUrl { get; set; }

        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }

    }
}
