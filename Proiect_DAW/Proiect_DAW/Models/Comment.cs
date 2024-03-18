using System.ComponentModel.DataAnnotations;

namespace Proiect_DAW.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        // cometariul este lasat de un user, TODO: de adaugat user

        [Required(ErrorMessage = "Comment content is required")]
        [MaxLength(200, ErrorMessage = "Comment content cannot be longer than 200 characters")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public int? ProjectId { get; set; }

        public virtual Project? Project { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public string? UserId { get; set; }

    }
}