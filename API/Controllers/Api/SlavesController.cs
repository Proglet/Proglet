using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Services;
using CoreDataORM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proglet.Core.Data;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlavesController : ControllerBase
    {
        private readonly DataContext _context;
        private IDockerService dockerService;

        public SlavesController(DataContext context, IDockerService dockerService)
        {
            _context = context;
            this.dockerService = dockerService;
        }

        public class RegisterData
        {
            public string Url { get; set; }
            public string Auth { get; set; }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterData registerData)
        {
            var slaveManager = _context.SlaveManagers.Where(s => s.Url == registerData.Url).FirstOrDefault();
            if(slaveManager == null)
            {
                slaveManager = new SlaveManager()
                {
                    Url = registerData.Url,
                    Auth = registerData.Auth,
                    Enabled = false
                };
                _context.SlaveManagers.Add(slaveManager);
                await _context.SaveChangesAsync();
            }


            if (slaveManager.Enabled)
            {
                dockerService.RegisterSlaveManager(slaveManager.Url);
                return Ok("ok");
            }
            else
                return Problem("Not allowed");
        }

        [HttpPost("callback")]
        public async Task<IActionResult> Callback(string id)
        {
            MemoryStream ms = new MemoryStream();
            await Request.Body.CopyToAsync(ms);

            dockerService.Callback(id, ms.ToArray(), _context);
            return Ok("ok");
        }


    }
}