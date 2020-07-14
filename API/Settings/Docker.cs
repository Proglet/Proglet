using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Settings
{
    /// <summary>
    /// Settings for docker slavemanagers
    /// </summary>
    public class Docker
    {
        /// <summary>
        /// The URL of the API that docker slavemanagers will post results back to
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Pending>")]
        public string CallbackUrl { get; set; }
    }
}
