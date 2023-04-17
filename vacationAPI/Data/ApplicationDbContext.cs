using Microsoft.EntityFrameworkCore;
using VacationAPI.Models;

namespace VacationAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<VacationRequest> VacationRequests { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the table names and relationships for the database schema
            modelBuilder.Entity<VacationRequest>().ToTable("VacationRequests");
            modelBuilder.Entity<User>().ToTable("Users");
        }
    }
}