using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NomRentals.Api.Entities;

namespace NomRentals.Api.Data
{
    public class CustomerApiDbContext : IdentityDbContext<UserProfile>
    {
        public CustomerApiDbContext(DbContextOptions options): base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Cook> Cooks { get; set; }
        public DbSet<ServiceBoy> ServiceBoys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // OnModelCreatingPartial(modelBuilder);
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    ConcurrencyStamp = "1"
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "2"
                });
        }


    }
}
