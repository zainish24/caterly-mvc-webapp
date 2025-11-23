using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models; // Ensure this namespace matches your models folder

namespace WebApplication1.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer name is required.")]
        [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters.")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string CustomerEmail { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Invalid phone number format.")]
        public string CustomerPhone { get; set; }

        [Required(ErrorMessage = "Reservation date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime ReservationDate { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
        public string Address { get; set; }

        public int MenuId { get; set; }
        public int CatererId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
        public string? Status { get; set; }

        // Foreign keys
        public Caterer Caterer { get; set; }

        public CatererCategory Category { get; set; }

        public Menu Menu { get; set; }

        public UserModel User { get; set; }

    }



    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateValue && dateValue <= DateTime.Now)
            {
                return new ValidationResult(ErrorMessage ?? "Date must be in the future.");
            }
            return ValidationResult.Success;
        }
    }
}








