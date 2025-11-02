using Microsoft.EntityFrameworkCore;
using User.API.Models;
namespace User.API.DAL
{

    public class userportaldbcontext : DbContext
    {
        public userportaldbcontext(DbContextOptions<userportaldbcontext> options) : base(options)
        {

        }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<RevokedToken> RevokedTokens { get; set; }
        public DbSet<UserLogin> Usersofuserportal { get; set; }
        //public DbSet<Order> Orders { get; set; }
        //public DbSet<OrderItem> OrderItems { get; set; }
        //public DbSet<OrderAddress> OrderAddresses { get; set; }
        //public DbSet<OrderPayment> OrderPayments { get; set; }
        //public DbSet<OrderStatusHistory> OrderStatusHistory { get; set; }

    }
}