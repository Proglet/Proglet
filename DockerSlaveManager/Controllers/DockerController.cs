using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DockerSlaveManager.Models.Multipart;
using DockerSlaveManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DockerSlaveManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DockerController : ControllerBase
    {
        private DockerService dockerService;

        public DockerController(DockerService dockerService)
        {
            this.dockerService = dockerService;
        }

        [HttpPost("run")]
        public IActionResult Run([FromForm] RunConfig runConfig)
        {
            if (runConfig.Image == null || !runConfig.Image.StartsWith("proglet/"))
                return Problem("Image not allowed");
            var id = dockerService.RunContainerBackground(runConfig);
            return Ok(id);
        }

        [HttpGet("queue")]
        public IActionResult Queue()
        {
            return Ok(dockerService.Queue());
        }

     

    }
}