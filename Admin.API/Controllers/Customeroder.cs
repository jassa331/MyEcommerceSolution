using Admin.API.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerOrderController : ControllerBase
    {
        private readonly admindbcontext _context;

        public CustomerOrderController(admindbcontext context)
        {
            _context = context;
        }

        [HttpGet("get-customer-orders")]
        public async Task<IActionResult> GetCustomerOrders()
        {
            // ✅ Get Admin ID from JWT
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid adminId))
                return Unauthorized("Invalid or missing UserId claim.");

            // ✅ Fetch orders with address
            var orders = await _context.Orders
                .Include(o => o.Product)
                .Include(o => o.OrderAddresses)
                .Where(o => o.Product.Usersid == adminId)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderNumber,
                    o.TotalAmount,
                    o.PaymentStatus,
                    o.OrderStatus,
                    o.CreatedAt,
                    ProductName = o.Product.Name,
                    CustomerId = o.AppUserId,
                    Addresses = o.OrderAddresses.Select(a => new
                    {
                        a.FullName,
                        a.Line1,
                        a.Line2,
                        a.City,
                        a.State,
                        a.PostalCode,
                        a.Country,
                        a.Phone,
                        a.AddressType
                    })
                })
                .ToListAsync();

            return Ok(orders);
        }
    }
}
