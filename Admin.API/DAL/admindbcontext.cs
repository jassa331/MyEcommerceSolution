using Admin.API.Models;
using Admin.API.NewFolder;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Admin.API.DAL
{
    public class admindbcontext : DbContext
    {



        public admindbcontext(DbContextOptions<admindbcontext> options) : base(options)
        {

        }
        public DbSet<OrderAddress> OrderAddresses { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ApiLog> ApiLogs { get; set; }
        public DbSet<manageorders> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AdminLogin> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<manageorders>()
           .HasKey(o => o.OrderId);

            // Address PK
            modelBuilder.Entity<OrderAddress>()
                .HasKey(a => a.OrderAddressId);

            // Order Item PK
            modelBuilder.Entity<OrderItem>()
                .HasKey(i => i.OrderItemId);
            base.OnModelCreating(modelBuilder);
        }
    }
}

