using Authentication.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.DAL
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<Userr> Userr { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Userr>().ToTable("Userr"); // 👈 ensures EF matches SQL
        }
    }
}
