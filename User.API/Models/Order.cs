
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User.API.Models
{
    public class Order
    {
        [ForeignKey("Order")]   
        public Guid OrderId { get; set; } = Guid.NewGuid();
        public string OrderNumber { get; set; } = string.Empty;
        public Guid AppUserId { get; set; }        // Buyer
        public Guid UsersId { get; set; }          // Seller
        public decimal TotalAmount { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public string OrderStatus { get; set; } = "New";
        public string? PaymentMethod { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
        public ICollection<OrderAddress>? OrderAddresses { get; set; }
        public ICollection<OrderPayment> OrderPayments { get; set; }
        public ICollection<OrderStatusHistory>? OrderStatusHistory { get; set; }
    }

    public class OrderItem
    {
        public Guid OrderItemId { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public Guid UsersId { get; set; }
        public Guid AppUserId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal RowTotal => UnitPrice * Quantity;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Order? Order { get; set; }
    }
    public class OrderAddress
    {
        public Guid OrderAddressId { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public string AddressType { get; set; } = "Shipping";
        public string FullName { get; set; } = string.Empty;
        public string Line1 { get; set; } = string.Empty;
        public string? Line2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = "India";
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Order? Order { get; set; }
    }


public class OrderPayment
    {
        [Key]
        public Guid PaymentId { get; set; } = Guid.NewGuid();

        [ForeignKey("Order")]
        public Guid OrderId { get; set; }

        public string PaymentProvider { get; set; } = "COD";
        public string? ProviderPaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string Status { get; set; } = "Pending";
        public string? ResponseRaw { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Order? Order { get; set; }
    }

    public class OrderStatusHistory
    {
        [Key] // ✅ This is the fix
        public Guid HistoryId { get; set; } = Guid.NewGuid();

        [ForeignKey("Order")]
        public Guid OrderId { get; set; }

        public string? FromStatus { get; set; }
        public string ToStatus { get; set; } = "New";
        public Guid? ChangedBy { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Order? Order { get; set; }
    }

}

