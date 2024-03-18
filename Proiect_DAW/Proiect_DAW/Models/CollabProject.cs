using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect_DAW.Models
{
    public class CollabProject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int? ProjectId { get; set; }

        public virtual Project Project { get; set; }
    }
}
