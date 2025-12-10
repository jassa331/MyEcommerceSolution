namespace Admin.API.Models
{
    public class manageorders
    {
        public Guid OrderId { get; set; }
        public Guid Usersid { get; set; }
        public Guid AppUserId { get; set; }
        public Guid ProductId { get; set; }
        public Guid CartItemId { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal TaxAmount { get; set; }

        public string OrderNumber { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentMethod { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public DateTime? CreatedAt { get; set; }

    }
    public class OrderAddress
    {
        public Guid? OrderAddressId { get; set; }
        public Guid OrderId { get; set; }
        public string? AddressType { get; set; }
        public string? FullName { get; set; }
        public string? Line1 { get; set; }
        public string? Line2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    public class OrderItem
    {
        public Guid? OrderItemId { get; set; }
        public Guid OrderId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? UsersId { get; set; }
        public Guid? AppUserId { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    public class OrderFullDto
    {
        public manageorders Order { get; set; }
        public List<OrderItem> Items { get; set; }
        public List<OrderAddress> Addresses { get; set; }
    }
    public class OrderStatusChartDto
    {
        public string Label { get; set; }
        public int Count { get; set; }
    }
    public class DashboardStatsDto
    {
        public int TotalActiveProducts { get; set; }
        public int TotalInactiveProducts { get; set; }
        public int OnlinePayments { get; set; }
        public int CodPayments { get; set; }
    }
}
