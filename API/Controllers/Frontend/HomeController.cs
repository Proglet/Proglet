using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.ORM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.Frontend
{
    public class HomeController : Controller
    {
        private readonly DataContext context;

        public HomeController(DataContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View(new API.Models.Frontend.Home.Index()
            {
                Courses = context.Courses.Include(c => c.CourseTemplate).ToList()
            });
        }
    }
}
