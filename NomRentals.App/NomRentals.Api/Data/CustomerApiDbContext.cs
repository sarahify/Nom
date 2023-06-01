using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NomRentals.Api.Entities;
using NomRentals.Api.Models;

namespace NomRentals.Api.Data
{
    public class CustomerApiDbContext : IdentityDbContext<UserProfile>
    {
        public CustomerApiDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> UserRoles { get; set; }  


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
                    Name = "Facilitator",
                    NormalizedName = "FACILITATOR",
                    ConcurrencyStamp = "2"
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "3"
                },
                new IdentityRole
                {
                    Name = "Cook",
                    NormalizedName = "COOK",
                    ConcurrencyStamp = "4"
                },
                new IdentityRole
                {
                    Name = "ServicesBoy",
                    NormalizedName = "SERVICESBOY",
                    ConcurrencyStamp = "5"
                },
                 new IdentityRole
                 {
                     Name = "SmallChops",
                     NormalizedName = "SMALLCHOPS",
                     ConcurrencyStamp = "6"
                 },
                new IdentityRole
                {
                    Name = "Decorations",
                    NormalizedName = "DECORATIONS",
                    ConcurrencyStamp = "7"
                },
                new IdentityRole
                {
                    Name = "Drinks",
                    NormalizedName = "DRINKS",
                    ConcurrencyStamp = "8"
                },
                 new IdentityRole
                 {
                     Name = "Cakes",
                     NormalizedName = "CAKES",
                     ConcurrencyStamp = "9"
                 });
        }


    }
}