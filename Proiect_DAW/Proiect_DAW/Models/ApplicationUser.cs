using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Proiect_DAW.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect_DAW.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<UserProject>? Projects { get; set; }

        public virtual ICollection<CollabProject>? CollabProjects { get; set; }

        public virtual ICollection<TaskAsignee>? Tasks { get; set; }

        public virtual ICollection<Note>? Notes { get; set; }

        //public string? FirstName { get; set; }

        // public string? LastName { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }
    }
}
