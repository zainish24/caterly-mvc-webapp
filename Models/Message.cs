namespace WebApplication1.Models
{
    using System.ComponentModel.DataAnnotations;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message content is required.")]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters.")]
        public string Messages { get; set; }

        public Caterer Caterer { get; set; }

        public CatererCategory Category { get; set; }

        public UserModel User { get; set; }

        [Required(ErrorMessage = "Caterer ID is required.")]
        public int CatererId { get; set; }

        [Required(ErrorMessage = "User  ID is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Category ID is required.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Created date is required.")]
        public DateTime CreatedDate { get; set; }

        public string? SenderType { get; set; }


        // Constructor to set the CreatedDate automatically
        public Message()
        {
            CreatedDate = DateTime.Now; // Set the current date and time
        }

    }
}