using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }

        // Full Name of the user
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, ErrorMessage = "Full Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        // Email Address
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        // User's Password
        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Confirm Password (used for validation, but won't be stored)
        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        // Contact Number
        [Required(ErrorMessage = "Contact number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string Contact { get; set; }

        // Gender (Optional)
        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        // Address of the user
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot be longer than 200 characters.")]
        public string Address { get; set; }

        public string? Status { get; set; }

    }
}
