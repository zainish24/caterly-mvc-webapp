using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class CatererLoginModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Caterer ID is required.")]
        public int CatererId { get; set; }
    }
}
