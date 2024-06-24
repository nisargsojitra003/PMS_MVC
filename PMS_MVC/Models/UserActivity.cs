using System.ComponentModel.DataAnnotations;

namespace PMS_MVC.Models
{
    public class UserActivity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        [StringLength(150)]
        public string Description { get; set; } = null!;

        public int UserId { get; set; }
    }
}
