using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public SlavesController(DataContext context)
        {
            _context = context;
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
                return Ok("ok");
            else
                return Problem("Not allowed");
        }


    }
}