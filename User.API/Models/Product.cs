using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace User.API.Models
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }

        [Required]
        public Guid usersid { get; set; }  // FK -> UserLogin.Id

        // Navigation property
        [ForeignKey(nameof(usersid))]
        public UserLogin? User { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool InStock { get; set; }
    }

    public class ProductUploadDto
    {
        public IFormFile File { get; set; } = null!;
        public string ProductName { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
    }
    public class CartItemDto
    {
        public Guid UsersofuserportalID { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string ProductDescription { get; set; } = "";
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; } = 1;
        public string ProductImageUrl { get; set; } = "";
    }

    // Entity
    public class CartItem
    {
        [JsonIgnore]
        public Guid CartItemID { get; set; }
        public Guid UsersofuserportalID { get; set; }  // 👈 buyer (from token)
        public Guid usersid { get; set; }               // 👈 seller (from Product)
        public Guid ProductId { get; set; }             // 👈 FK to Product

        public string ProductName { get; set; } = string.Empty;
        public string? ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public string? ProductImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}

