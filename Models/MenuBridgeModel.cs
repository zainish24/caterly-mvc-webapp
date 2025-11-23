using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class MenuBridgeModel
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "Menu title must be under 100 characters.")]
        public string MenuTitle { get; set; }

        [StringLength(1000, ErrorMessage = "Menu description must be under 1000 characters.")]
        public string MenuDescription { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required]
        public IFormFile MenuImage { get; set; }

        [Required(ErrorMessage = "Number of people is required.")]
        [Range(50, 200, ErrorMessage = "Number of people must be between 50 and 200.")]
        public int NumberOfPeople { get; set; }

        [Required(ErrorMessage = "Caterer is required.")]
        public int CatererId { get; set; }


    }
}
