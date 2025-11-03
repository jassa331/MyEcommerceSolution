using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Security.Claims;
using User.API.DAL;
using User.API.Models;

namespace User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly userportaldbcontext _context;

        public OrdersController(userportaldbcontext context)
        {
            _context = context;
        }

        // 🧾 GET: Get All Orders by Logged-in User
        [HttpGet("MyOrders")]
        public async Task<IActionResult> GetMyOrders()
        {
            // Try to find the "UserId" claim from your JWT token
            var userIdClaim = User.FindFirst("UserId")?.Value;

            // Fallback: if token uses "sub" or "nameidentifier"
            if (string.IsNullOrEmpty(userIdClaim))
                userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Validate GUID
            if (!Guid.TryParse(userIdClaim, out Guid uid))
                return Unauthorized("Invalid or missing UserId in token.");

            // Fetch orders for this user
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.OrderAddresses)
                .Include(o => o.OrderPayments)
                .Where(o => o.AppUserId == uid && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders);
        }


        // 🛒 POST: Create Order (from Cart)
        [HttpPost("Create")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID missing in token.");

            order.AppUserId = Guid.Parse(userId);
            order.OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{new Random().Next(10000, 99999)}";
            order.CreatedAt = DateTime.Now;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order placed successfully!", orderId = order.OrderId });
        }

        // 📦 GET: Order by Id
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.OrderAddresses)
                .Include(o => o.OrderPayments)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return NotFound("Order not found");

            return Ok(order);
        }
    }
}
