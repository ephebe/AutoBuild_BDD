using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Persistence
{
    public class BDDContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public BDDContext(DbContextOptions<BDDContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable("Course");
        }
    }
}
