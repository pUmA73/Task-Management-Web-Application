using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proiect_DAW.Data;
using Proiect_DAW.Models;

namespace Proiect_DAW.Controllers
{
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public NotesController(ApplicationDbContext context,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult New(Note note)
        {
            note.Date = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Note.Add(note);
                db.SaveChanges();
                return Redirect("/Tasks/Show/" + note.TaskId);
            }
            else
            {
                return Redirect("/Tasks/Show/" + note.TaskId);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Delete(int id)
        {
            Note note = db.Note.Find(id);
            if (note.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Note.Remove(note);
                db.SaveChanges();
                return Redirect("/Tasks/Show/" + note.TaskId);
            }
            else
            {
                return Redirect("/Tasks/Show/" + note.TaskId);
            }


        }

        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Edit(int id)
        {
            Note note = db.Note.Find(id);
            if (note.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(note);
            }
            else
            {
                return Redirect("/Tasks/Show/" + note.TaskId);
            }

        }

        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Edit(int id, Note requestNote)
        {
            Note note = db.Note.Find(id);

            if (ModelState.IsValid)
            {
                if (note.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    note.Date = DateTime.Now;
                    note.Content = requestNote.Content;
                    db.SaveChanges();

                    return Redirect("/Tasks/Show/" + note.TaskId);
                }
                else
                {
                    return Redirect("/Tasks/Show/" + note.TaskId);
                }

            }
            else
            {
                return View(requestNote);
            }
        }
    }
}
