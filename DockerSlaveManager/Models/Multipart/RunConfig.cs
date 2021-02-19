using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DockerSlaveManager.Models.Multipart
{
    public class RunConfig
    {
        public string Image { get; set; }
        public Dictionary<string, string> Environment { get; set; }
        public string CallbackUrl { get; set; }
        public IFormFile ZipOverlay { get; set; }
        public Dictionary<string, string> Mounts { get; set; } = new Dictionary<string, string>();

        public string OutPath { get; set; } = "/app/out";
    }
}
