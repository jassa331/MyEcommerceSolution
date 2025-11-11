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


        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<OrderAddress> OrderAddresses { get; set; }

        public DbSet<AdminLogin> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ProductId);

            modelBuilder.Entity<OrderAddress>()
                .HasOne(a => a.Order)
                .WithMany(o => o.OrderAddresses)
                .HasForeignKey(a => a.OrderId);

            base.OnModelCreating(modelBuilder);
        }


    }
}
