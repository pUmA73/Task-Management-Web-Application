using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect_DAW.Data;
using Proiect_DAW.Models;
using System.Linq;

namespace Proiect_DAW.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<ProjectsController> logger)
        {
            db = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Index()
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
            //TODO: include the user table when it is made
            var projects = db.Projects
                            .Where(pro => pro.Collaborators.Any(user => user.UserId == _userManager.GetUserId(User))
                             || pro.Organizers.Any(user => user.UserId == _userManager.GetUserId(User)));


            //how do i check if there are organizers or orgaorators

            ViewBag.Projects = projects;
            return View();
        }

        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Show(int id)
        {
            //probably here we will need to include more tables like user and comments
            var project = db.Projects.Include("Comments").Include("Tasks").Include("Organizers").Include("Collaborators")
                                     .Where(proj => proj.Id == id)
                                     .First();
            ViewBag.AfisareButoane = false;
            ViewBag.Collaborators = db.CollabProjects.Include("User").Where(collab => collab.ProjectId == id);

            if (project.Organizers.Any(orga => orga.UserId == _userManager.GetUserId(User)) || User.IsInRole("Admin"))
            {
                ViewBag.AfisareButoane = true;

            }

            return View(project);
        }
        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);

                db.SaveChanges();
                ViewBag.AfisareButoane = false;
                return Redirect("/Projects/Show/" + comment.ProjectId);
            }
            else
            {
                Project proje = db.Projects.Include("Tasks").Include("Comments").Include("Organizers").Include("Collaborators")
                    .Where(proje => proje.Id == comment.ProjectId)
                    .First();

                return View(proje);
            }
        }
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult New()
        {

            Project project = new Project();
            return View(project);
        }

        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public async Task<IActionResult> New(Project project)
        {
            //Uncomment this when validations are done
            if (ModelState.IsValid)
            {
                project.State = "In progress";
                project.Deadline = DateTime.Now.AddDays(7);

                // adaugam id-ul userului curent intr-un obiect
                // de tip UserProject care face legatura dintre
                // user si project
                db.Projects.Add(project);

                db.SaveChanges();
                //await db.SaveChangesAsync();
                UserProject aux = new UserProject();
                aux.UserId = _userManager.GetUserId(User);
                aux.ProjectId = project.Id;

                db.UserProjects.Add(aux);
                db.SaveChanges();
                // await db.SaveChangesAsync();

                // dupa ce un user a creat un proiect 
                // este facut automat si organizator

                //ApplicationUser user = db.Users.Find(_userManager.GetUserId(User)); 
                //_userManager.AddToRoleAsync(user, "Organizer");

                if(!User.IsInRole("Admin"))
                {
                    await UpdateCurrentUser("Organizer", _userManager.GetUserId(User));
                }

                // TODO - if role is user 
                return RedirectToAction("Index");
            }
            else
                return View(project);
        }

        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult Edit(int id)
        {
            var project = db.Projects.Include("Collaborators").Include("Organizers")
                            .Where(proj => proj.Id == id)
                            .First();
            if (project.Organizers.Any(orga => orga.UserId == _userManager.GetUserId(User)) || User.IsInRole("Admin"))
            {
                return View(project);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult Edit(int id, Project requestProject)
        {
            Project project = db.Projects.Include("Organizers")
                                .Where(pro => pro.Id == id).First();
            //Uncomment this when validations are done
            if (ModelState.IsValid)
            {
                if (project.Organizers.Any(orga => orga.UserId == _userManager.GetUserId(User)) || User.IsInRole("Admin"))
                {
                    project.Name = requestProject.Name;
                    project.Description = requestProject.Description;
                    project.Deadline = requestProject.Deadline;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
                return View(requestProject);
        }

        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            Project project = db.Projects
                              .Include("Comments")
                              .Include("Tasks")
                              .Include("Organizers")
                              .Include("Collaborators")
                              .Include("Tasks.Notes")
                              .Include("Tasks.Assignees")
                              .Where(pro => pro.Id == id)
                              .First();

            ApplicationUser user = db.Users
                            .Include("Projects")
                            .Where(u => u.Id == _userManager.GetUserId(User))
                            .First();

            if (project.Organizers.Any(orga => orga.UserId == _userManager.GetUserId(User)) || User.IsInRole("Admin"))
            {

                var organizer = project.Organizers.First();

                // stergem din tabela asociativa
                var conn = db.UserProjects
                            .Where(up => up.UserId == organizer.UserId);
                if (conn.Count() == 1)
                {
                    foreach (UserProject item in conn)
                    {
                        db.UserProjects.Remove(item);
                    }
                    db.SaveChanges();
                }


                //db.CollabProjects.RemoveRange(project.Collaborators);
                //db.UserProjects.RemoveRange(project.Organizers);
                db.Comments.RemoveRange(project.Comments);
                db.Projects.Remove(project);
                db.Note.RemoveRange(project.Tasks.SelectMany(task => task.Notes));
                db.TaskAsignees.RemoveRange(project.Tasks.SelectMany(task => task.Assignees));
                db.Tasks.RemoveRange(project.Tasks);

                db.SaveChanges();

                await RetrogadeOrganizer(organizer.UserId);

                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult AddCollaborator(string userId, int projectId)
        {
            CollabProject orga = new CollabProject();
            orga.UserId = userId;
            orga.ProjectId = projectId;


            Project project = db.Projects.Include("Collaborators")
                              .Where(pro => pro.Id == projectId)
                              .First();

            project.Collaborators.Add(orga);

            if (ModelState.IsValid)
            {

                /*if (project.Collaborators.Any(orga => orga.UserId == _userManager.GetUserId(User)) || User.IsInRole("Admin"))
                {*/
                //aici verifica daca id-ul colaboratorului este id-ul userului curent ????
                db.CollabProjects.Add(orga);
                db.SaveChanges();
                return Redirect("/Projects/Show/" + projectId);
                /* }
                 else
                 {
                     TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                     TempData["messageType"] = "alert-danger";
                     return RedirectToAction("Index");
                 }*/
            }
            else
            {
                return Redirect("/Projects/Show/" + projectId);
            }
        }

        public async System.Threading.Tasks.Task UpdateCurrentUser(string roleName, string userId)
        {
            var user = db.Users.Where(us => us.Id == userId).First();

            // rolul dat ca parametru nu exista
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                _logger.LogWarning("Role {roleName} does not exist", roleName);
                return;
            }

            if (user == null)
            {
                return;
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                db.SaveChanges();
                //await db.SaveChangesAsync();
                await _signInManager.RefreshSignInAsync(user);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError($"Error adding user to role: {error.Description}");
                }

            }
        }

        // Functie pentru a retrograda organizatorul daca acesta isi sterge
        // ultimul proiect
        private async System.Threading.Tasks.Task RetrogadeOrganizer(string userId)
        {
            var user = db.Users.Where(us => us.Id == userId).First();

            if(user == null)
            {
                return;
            }

            var result = await _userManager.RemoveFromRoleAsync(user, "Organizer");

            if(result.Succeeded)
            {
                db.SaveChanges();
            }
        }

        public IActionResult UserRedirect()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
