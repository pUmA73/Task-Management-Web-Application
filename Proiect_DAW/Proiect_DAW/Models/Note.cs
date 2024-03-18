using System.ComponentModel.DataAnnotations;

namespace Proiect_DAW.Models
{
    public class Note
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Note content is required")]
        [MaxLength(200, ErrorMessage = "Note content cannot be longer than 200 characters")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        // o notita apartine unui task - este FK
        public int? TaskId { get; set; }

        public virtual Task? Task { get; set; }

        // TODO - de implementat partea cu userii
        public virtual ApplicationUser? User { get; set; }

        public string? UserId { get; set; }
    }
}