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
        [HttpPost("Create")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            var userIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User ID missing in token.");

            Guid userId = Guid.Parse(userIdClaim);

            // ✅ Always create new OrderId before adding to context
            order.OrderId = Guid.NewGuid();
            order.AppUserId = userId;
            order.OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{new Random().Next(10000, 99999)}";
            order.CreatedAt = DateTime.Now;

            // ✅ Fix child relationships
            if (order.OrderItems != null)
            {
                foreach (var item in order.OrderItems)
                {
                    item.OrderId = order.OrderId;
                    item.AppUserId = order.AppUserId;
                }
            }

            if (order.OrderAddresses != null)
            {
                foreach (var address in order.OrderAddresses)
                {
                    address.OrderId = order.OrderId;
                }
            }

            if (order.OrderPayments != null)
            {
                foreach (var pay in order.OrderPayments)
                {
                    pay.OrderId = order.OrderId;
                }
            }

            if (order.OrderStatusHistory != null)
            {
                foreach (var history in order.OrderStatusHistory)
                {
                    history.OrderId = order.OrderId;
                }
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order placed successfully!", orderId = order.OrderId });
        }

        //[HttpPost("Create")]
        //public async Task<IActionResult> Create([FromBody] Order order)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (order.OrderItems == null || !order.OrderItems.Any())
        //    {
        //        return BadRequest("Order must contain items.");
        //    }

        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();
        //    return Ok(new { Message = "Order created successfully", OrderId = order.OrderId });
        //}


        // 🛒 POST: Create Order (from Cart)
        //[HttpPost("Create")]
        //public async Task<IActionResult> CreateOrder([FromBody] Order order)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized("User ID missing in token.");

        //    order.AppUserId = Guid.Parse(userId);
        //    order.OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{new Random().Next(10000, 99999)}";
        //    order.CreatedAt = DateTime.Now;

        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "Order placed successfully!", orderId = order.OrderId });
        //}
        //[HttpPost("Create")]
        //public async Task<IActionResult> CreateOrder([FromBody] Order order)
        //{
        //    // ✅ Extract UserId claim (not ClaimTypes.NameIdentifier)
        //    var userIdClaim = User.FindFirstValue("UserId");
        //    if (string.IsNullOrEmpty(userIdClaim))
        //        return Unauthorized("User ID missing in token.");

        //    Guid userId = Guid.Parse(userIdClaim);

        //    order.AppUserId = userId;
        //    order.OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{new Random().Next(10000, 99999)}";
        //    order.CreatedAt = DateTime.Now;

        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "Order placed successfully!", orderId = order.OrderId });
        //}




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
