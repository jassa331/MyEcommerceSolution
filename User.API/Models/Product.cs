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
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
    public class OrderAddressDto
    {
        public string FullName { get; set; } = "";
        public string Line1 { get; set; } = "";
        public string? Line2 { get; set; }
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string Country { get; set; } = "India";
        public string Phone { get; set; } = "";
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

        public Guid Appuserid { get; set; }   // ✅ Buyer (from JWT)
        public Guid usersid { get; set; }     // ✅ Seller (from product)
        public Guid ProductId { get; set; }   // ✅ FK to Product

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }


}

