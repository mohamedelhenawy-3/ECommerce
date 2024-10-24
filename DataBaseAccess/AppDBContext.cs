
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModelClasses;
using static ModelClasses.ApplicationUser;


namespace DataBaseAccess
{
    public class AppDBContext : IdentityDbContext<ApplicationUser>
    {
    
        public DbSet<ApplicationUser> AppUser { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<PImages> PImages { get; set; }
        public DbSet<UserCart> UserCarts { get; set; }
        public DbSet<UserOrderHeader> UserOrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
    }
}
