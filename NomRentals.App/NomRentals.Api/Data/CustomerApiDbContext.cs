using Microsoft.EntityFrameworkCore;
using NomRentals.Api.Entities;

namespace NomRentals.Api.Data
{
    public class CustomerApiDbContext : DbContext
    {
        public CustomerApiDbContext(DbContextOptions options): base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Cook> Cooks { get; set; }
        public DbSet<ServiceBoy> ServiceBoys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
