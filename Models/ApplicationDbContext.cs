using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<CatererCategory> CatererCategories { get; set; }
        public DbSet<Caterer> Caterers { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Message> Messages { get; set; }



    }

   
}
