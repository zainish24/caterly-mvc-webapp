using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class CatererBridgeModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Caterer name is required.")]
        [StringLength(100, ErrorMessage = "Caterer name must be under 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description must be under 500 characters.")]
        public string Description { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(15, ErrorMessage = "Phone number must be under 15 characters.")]
        public string Phone { get; set; } // Optional


        [Required(ErrorMessage = "Image is required.")]
        public IFormFile CatererImage { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; } // Foreign key reference

        [StringLength(255, ErrorMessage = "Location must be under 255 characters.")]
        public string Location { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(100, ErrorMessage = "Email must be under 100 characters.")]
        public string Email { get; set; }

        
    }
}
