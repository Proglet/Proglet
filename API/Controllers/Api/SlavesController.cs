using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Models.API;
using API.ORM;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proglet.Core.Data;

namespace API.Controllers
{

    /// <summary>
    /// Controller to send commands to Docker instances
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SlavesController : ControllerBase
    {
        private readonly DataContext _context;
        private IDockerService dockerService;

        /// <summary>
        /// Constructor called using DI
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dockerService"></param>
        public SlavesController(DataContext context, IDockerService dockerService)
        {
            _context = context;
            this.dockerService = dockerService;
        }


        /// <summary>
        /// API call to register a docker slave manager. Should contain registration info. If this slave is registered, and the key is valid, the manager is accepted
        /// </summary>
        /// <param name="registerData"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]DockerSlaveRegisterData registerData)
        {
            if (registerData == null || registerData.Url == null || registerData.Auth == null)
                return Problem("No registration data");
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
            using (MemoryStream ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                dockerService.Callback(id, ms.ToArray(), _context);
            }
            return Ok("ok");
        }


    }
}