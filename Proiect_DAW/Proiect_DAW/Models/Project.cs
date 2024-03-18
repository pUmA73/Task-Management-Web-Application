using Proiect_DAW.Custom_Validation;
using System.ComponentModel.DataAnnotations;

namespace Proiect_DAW.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Project name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Project description is required")]
        [MinLength(10, ErrorMessage = "Project description must be at least 10 characters long")]
        [MaxLength(1000, ErrorMessage = "Project description cannot be longer than 1000 characters")]
        public string Description { get; set; }

        public virtual ICollection<Task>? Tasks { get; set; }

        // posibil sa devina ICollection in viitor
        public virtual ICollection<UserProject>? Organizers { get; set; }

        public virtual ICollection<CollabProject>? Collaborators { get; set; }

        public string? State { get; set; }

        [Required(ErrorMessage = "Project deadline is required")]
        [CustomDeadline(ErrorMessage = "Project deadline must be in the future")]
        public DateTime? Deadline { get; set; }

        // public virtual ICollection </aici o sa vina de user/ > Collaborators { get; set;}

        public virtual ICollection<Comment>? Comments { get; set; }


    }
}