using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class CatererCategory
    {
        [Key]

        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100)]
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<Caterer> Caterers { get; set; } = new List<Caterer>();


    }
}
