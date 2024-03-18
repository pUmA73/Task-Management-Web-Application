using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect_DAW.Data;
using Proiect_DAW.Models;
using System.Diagnostics;

namespace Proiect_DAW.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly ILogger<ProjectsController> _logger;

        public HomeController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<ProjectsController> logger)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult IndexLogged()
        {
            if (User.IsInRole("Admin"))
            {
                var project = db.Projects
                            .Include("Comments")
                            .Include("Tasks")
                            .Include("Collaborators")
                            .Include("Organizers");
                ViewBag.Projects = project;
                return View();
            }
            
            var projects = db.Projects
                            .Where(pro => pro.Collaborators.Any(user => user.UserId == _userManager.GetUserId(User))
                             || pro.Organizers.Any(user => user.UserId == _userManager.GetUserId(User)));

            var tasks = db.TaskAsignees
                        .Where(ta => ta.UserId == _userManager.GetUserId(User))
                        .Select(ta => ta.Task)
                        .ToList();

            ViewBag.Tasks = tasks;

            int count = 0;

            foreach (var task in tasks) { count++; }

            ViewBag.TaskCount = count;

            ViewBag.CurrDate = DateTime.Now;

            ViewBag.Projects = projects;
            return View();
        }

        public IActionResult UserRedirect()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("IndexLogged");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}