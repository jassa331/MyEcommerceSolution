using System.ComponentModel.DataAnnotations;

namespace User.API.Models
{
    public class UserLogin
    {
        [Key]
        public Guid Id { get; set; }       // PK (used as FK in Product.usersid)
        public string Email { get; set; }
        public string Password { get; set; }

       
    }
    public class RevokedToken
    {
         public  Guid Id { get; set; }
        public string Token { get; set; }

        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow;
    }
}
