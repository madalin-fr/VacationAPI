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

    }
}
