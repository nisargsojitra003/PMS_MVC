using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PMS_MVC.Models
{
    public class Category
    {
        public int Id { get; set; }

        //[StringLength(50)]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(25, ErrorMessage = "Only 25 Characaters are Accepted")]
        [RegularExpression(@"^[a-zA-Z]+( [a-zA-Z]*)*$", ErrorMessage = "Name must start with an alphabetic character and contain one or more words with spaces between them.")]
        public string Name { get; set; } = null!;


        [Column(TypeName = "timestamp without time zone")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? ModifiedAt { get; set; }

        [StringLength(5)]
        [Required(ErrorMessage = "Code is required")]
        [RegularExpression(@"^\d{1,5}$", ErrorMessage = "The field must be between 1 and 5 digits.")]
        public string? Code { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? DeletedAt { get; set; }

        [StringLength(250, ErrorMessage = "Only 250 characters are accepted")]
        [Required(ErrorMessage = "Description is required")]
        [RegularExpression(@"^[a-zA-Z][\s\S]*$", ErrorMessage = "Description must start with an alphabet.")]
        public string? Description { get; set; }

        public int? UserId { get; set; }

        public bool? IsSystem { get; set; }
    }
}
