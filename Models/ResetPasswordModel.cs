namespace WebApplication1.Models
{
    public class ResetPasswordModel
    {
        public int Id { get; set; } // User ID
        public string CurrentPassword { get; set; } // Current password for verification
        public string NewPassword { get; set; } // New password
        public string ConfirmPassword { get; set; } // Confirmation of the new password
    }
}
