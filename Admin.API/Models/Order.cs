using Admin.API.NewFolder;
using System;
using System.ComponentModel.DataAnnotations;

namespace Admin.API.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; }
        public Guid AppUserId { get; set; }
        public Guid? ProductId { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public Product Product { get; set; }

        // ✅ New navigation
        public ICollection<OrderAddress> OrderAddresses { get; set; }
    }
    public class OrderAddress
    {
        [Key]
        public Guid OrderAddressId { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public string AddressType { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Line1 { get; set; }

        public string Line2 { get; set; }

        [Required]
        public string City { get; set; }

        public string State { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string Country { get; set; }

        public string Phone { get; set; }

        public DateTime CreatedAt { get; set; }

        // ✅ Navigation property
        public Order Order { get; set; }
    }
}
