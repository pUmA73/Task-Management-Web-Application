using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect_DAW.Data;
using Proiect_DAW.Models;
using Task = Proiect_DAW.Models.Task;

namespace Proiect_DAW.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public TasksController(ApplicationDbContext context,
                UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Index(int projectId)
        {
            var tasks = db.Tasks.Where(task => task.ProjectId == projectId);

            ViewBag.Tasks = tasks;
            ViewBag.ProjectId = projectId;

            return View();
        }

        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Show(int id)
        {
            Task task = db.Tasks.Include("Notes")
                                .Include("Assignees")
                                .Include("Project")
                                .Include("Project.Organizers")
                                .Include("Assignees.User")
                                .Where(task => task.Id == id)
                                .First();
            ViewBag.AfisareButoane = false;
            ViewBag.Assignees = task.Assignees;

            if (task.Project.Organizers.Any(orga => orga.UserId == _userManager.GetUserId(User)) || User.IsInRole("Admin"))
            {
                ViewBag.AfisareButoane = true;

            }

            return View(task);
        }

        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Show([FromForm] Note note)
        {
            note.Date = DateTime.Now;
            note.UserId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                db.Note.Add(note);
                db.SaveChanges();
                return Redirect("/Tasks/Show/" + note.TaskId);
            }
            else
            {
                Task task = db.Tasks.Include("Notes")
                                    .Where(task => task.Id == note.TaskId)
                                    .First();

                return View(task);
            }

        }

        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult New(int projectId)
        {
            Task task = new Task { ProjectId = projectId };
            Project project = db.Projects.Include("Collaborators")
                                         .Include("Organizers")
                                         .Where(project => project.Id == projectId)
                                         .First();

            if (project.Organizers.Any(orga => orga.UserId == _userManager.GetUserId(User)) || User.IsInRole("Admin"))

            {
                return View(task);
            }
            else
            {
                TempData["message"] = "You are not allowed to add tasks to this project";
                return Redirect("/Projects/Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult New(Task task)
        {
            Project project = db.Projects.Include("Collaborators").Include("Organizers")
                              .Where(project => project.Id == task.ProjectId)
                              .First();
            if (ModelState.IsValid)
            {
                if (project.Organizers.Any(orga => orga.UserId == _userManager.GetUserId(User)) || User.IsInRole("Admin"))
                {

                    db.Tasks.Add(task);
                    db.SaveChanges();
                    return Redirect("/Tasks/Index?projectId=" + task.ProjectId);
                }
                else
                {
                    TempData["message"] = "You are not allowed to add tasks to this project";
                    return Redirect("/Projects/Index");
                }
            }
            else
            {
                return View(task);
            }
        }

        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult Edit(int id)
        {
            Task task = db.Tasks.Where(task => task.Id == id)
                                .First();
            Project project = db.Projects.Include("Collaborators").Include("Organizers")
                              .Where(project => project.Id == task.ProjectId)
                              .First();
            if (project.Organizers.Any(orga => orga.UserId == _userManager.GetUserId(User)) || User.IsInRole("Admin"))
            {
                return View(task);
            }
            else
            {
                TempData["message"] = "You are not allowed to edit tasks from this project";
                return Redirect("/Projects/Index");
            }


        }

        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult Edit(int id, Task requestTask)
        {
            Task task = db.Tasks.Find(id);
            Project project = db.Projects.Include("Collaborators").Include("Organizers")
                              .Where(project => project.Id == task.ProjectId)
                              .First();

            if (ModelState.IsValid)
            {
                if (project.Organizers.Any(orga => orga.UserId == _userManager.GetUserId(User)) || User.IsInRole("Admin"))
                {
                    task.Title = requestTask.Title;
                    task.Description = requestTask.Description;
                    task.Status = requestTask.Status;
                    task.StartDate = requestTask.StartDate;
                    task.FinishDate = requestTask.FinishDate;
                    task.Content = requestTask.Content;
                    task.Images = requestTask.Images;
                    task.Videos = requestTask.Videos;
                    task.YoutubeVideos = requestTask.YoutubeVideos;

                    db.SaveChanges();
                    return Redirect("/Tasks/Index/Tasks?projectId=" + task.ProjectId);
                }
                else
                {
                    TempData["message"] = "You are not allowed to edit tasks from this project";
                    return Redirect("/Projects/Index");
                }
            }
            else
            {
                return View(requestTask);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult Delete(int id)
        {
            Task task = db.Tasks.Include("Notes")
                                .Where(task => task.Id == id)
                                .First();
            Project project = db.Projects.Include("Collaborators").Include("Organizers")
                              .Where(project => project.Id == task.ProjectId)
                              .First();
            if (project.Organizers.Any(orga => orga.UserId == _userManager.GetUserId(User)) || User.IsInRole("Admin"))
            {
                db.Note.RemoveRange(task.Notes);
                db.Tasks.Remove(task);
                db.SaveChanges();

                return Redirect("/Tasks/Index?projectId=" + task.ProjectId);
            }
            else
            {
                TempData["message"] = "You are not allowed to delete tasks from this project";
                return Redirect("/Projects/Index");
            }
        }

        public IActionResult AddAsignee(string userId, int taskId)
        {
            TaskAsignee ta = new TaskAsignee();
            ta.UserId = userId;
            ta.TaskId = taskId;


            Task task = db.Tasks.Include("Assignees")
                              .Where(ta => ta.Id == taskId)
                              .First();

            task.Assignees.Add(ta);

            if (ModelState.IsValid)
            {

                /*if (project.Collaborators.Any(orga => orga.UserId == _userManager.GetUserId(User)) || User.IsInRole("Admin"))
                {*/
                //aici verifica daca id-ul colaboratorului este id-ul userului curent ????
                db.TaskAsignees.Add(ta);
                db.SaveChanges();
                return Redirect("/Tasks/Show/" + taskId);
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
                return Redirect("/Tasks/Show/" + taskId);
            }
        }



    }
}
