using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proglet.Core.Data;
using API.Models.Multipart;
using API.ORM;

namespace API.Controllers
{
    /// <summary>
    /// Controller for exercises. Will be used later to update/download individual exercises
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        private readonly DataContext _context;

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        public ExercisesController(DataContext context)
        {
            _context = context;
        }




    }
}
