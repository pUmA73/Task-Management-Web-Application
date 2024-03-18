using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect_DAW.Models
{
    public class TaskAsignee
    {
        //entitate care sa sparga legatura many to many dintre Task si User

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int? TaskId { get; set; }

        public virtual Task Task { get; set; }
    }
}
