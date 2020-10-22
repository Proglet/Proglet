using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Proglet.Core.Data
{
    public class DockerTestImage
    {
        [Key]
        public int DockerTestImageId { get; set; }

        public string ImageName { get; set; }
        public string Description { get; set; }
    }
}
