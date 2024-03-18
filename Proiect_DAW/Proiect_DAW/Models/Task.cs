using Microsoft.AspNetCore.Mvc.Rendering;
using Proiect_DAW.Custom_Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
namespace Proiect_DAW.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Task title is required")]
        [MaxLength(30, ErrorMessage = "Task title cannot be longer than 30 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Task description is required")]
        [MinLength(10, ErrorMessage = "Task description must be at least 10 characters long")]
        [MaxLength(1000, ErrorMessage = "Task description cannot be longer than 1000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Task status is required")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Task start date is required")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Task finish date is required")]
        public DateTime? FinishDate { get; set; }

        [CustomDateRange("StartDate", "FinishDate", ErrorMessage = "Finish date must be greater than or equal to start date.")]
        public object? DateRangeValidation => null;

        [Required(ErrorMessage = "Task content is required")]
        [MinLength(10, ErrorMessage = "Task content must be at least 10 characters long")]
        [MaxLength(200, ErrorMessage = "Task content cannot be longer than 200 characters")]
        public string Content { get; set; }

        // un task apartine unui proiect - este FK
        public int? ProjectId { get; set; }

        public Project? Project { get; set; }

        public virtual ICollection<Note>? Notes { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<TaskAsignee>? Assignees { get; set; }


        public string? Text { get; set; }


        //@Html.Label("Images", "Add images to be displayed in the task")
        //        <br />
        //        <input type = "file" name="Images" id="Images" accept="image/*">
        public string? Images { get; set; }


        public string? Videos { get; set; }


        public string? YoutubeVideos { get; set; }

        [NotMapped]

        public IEnumerable<SelectListItem>? StatusOptions { get; } = new List<SelectListItem>
        {
            new SelectListItem { Text = "Not Started", Value = "Not Started" },
            new SelectListItem { Text = "In Progress", Value = "In Progress" },
            new SelectListItem { Text = "Completed", Value = "Completed" },
            new SelectListItem { Text = "On Hold", Value = "On Hold" }
        };
    }
}