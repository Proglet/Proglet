using API.ORM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers.Frontend
{
    [Route("[controller]")]
    [Controller]
    public class SubmissionController : Controller
    {
        private readonly DataContext context;

        public SubmissionController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ViewSubmission(int id)
        {
            return View(await 
                context.Submissions
                    .Include(s => s.User)
                    .Include(s => s.Exercise)
                    .Include(s => s.TestResults)
                        .ThenInclude(tr => tr.Test)
                            .ThenInclude(t => t.Point)
                    .FirstAsync(s => s.SubmissionId == id));
        }
    }
}
