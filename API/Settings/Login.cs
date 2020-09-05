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
        public Uri RequestUrl { get; set; }
        public Uri LoginUrl { get; set; }
        public Uri TokenUrl { get; set; }
        public Uri InfoUrl { get; set; }
        public Uri MoreInfoUrl { get; set; }

        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }

    }
}
