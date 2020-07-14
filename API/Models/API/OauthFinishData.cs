using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.API
{
    /// <summary>
    /// Data used to finish up an oath request 
    /// </summary>
    public class OauthFinishData
    {
        [Required]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
        public string oauth_token { get; set; }
        [Required]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
        public string oauth_verifier { get; set; }
        [Required]
        public string jwt { get; set; }
        [Required]
        public string loginservice { get; set; }
    }

}
