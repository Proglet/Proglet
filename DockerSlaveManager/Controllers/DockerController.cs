using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet("test")]
        public IActionResult test()
        {
            var id = dockerService.RunContainerBackground("proglet/projectparser-intellij-edutools");
            return Ok(id);
        }

        [HttpGet("status")]
        public IActionResult status(string id)
        {
            var status = dockerService.GetRunStatus(id);
            return Ok(status);
        }

    }
}