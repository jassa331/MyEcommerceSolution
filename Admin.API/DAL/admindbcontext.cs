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
        public DbSet<Product> Products { get; set; }
        public DbSet<AdminLogin> Users { get; set; }
    }
}
