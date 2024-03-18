using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect_DAW.Data;
using Proiect_DAW.Models;

namespace Proiect_DAW.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public CommentsController(ApplicationDbContext context,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                return Redirect("/Projects/Show/" + comm.ProjectId);
            }
            else
            {
                return Redirect("/Projects/Show/" + comm.ProjectId);
            }
        }

        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Edit(int id)
        {

            Comment comm = db.Comments.Find(id);
            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(comm);
            }
            else
            {
                return Redirect("/Projects/Show/" + comm.ProjectId);
            }

        }

        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comm = db.Comments.Find(id);
            if (ModelState.IsValid)
            {
                if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    comm.Content = requestComment.Content;

                    db.SaveChanges();

                    return Redirect("/Projects/Show/" + comm.ProjectId);
                }
                else
                {
                    return Redirect("/Projects/Show/" + comm.ProjectId);
                }


            }
            else
            {
                return View(requestComment);
            }
        }
    }
}
