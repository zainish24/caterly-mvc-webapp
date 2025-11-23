namespace WebApplication1.Models
{
    public class DataViewModel
    {
        public List<Caterer> Caterers { get; set; } = new List<Caterer>();
        public List<CatererCategory> CatererCategories { get; set; } = new List<CatererCategory>();
        public List<Menu> Menus { get; set; } = new List<Menu>();
        public List<UserModel> Users { get; set; } = new List<UserModel>();
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<Message> Messages { get; set; } = new List<Message>(); // Add this line
        public virtual Booking Booking { get; set; }
        public virtual Message Message { get; set; }
    }
}
