using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        [HttpPost("payment-success")]
        public async Task<IActionResult> PaymentSuccess([FromQuery] string session_id)
        {
            StripeConfiguration.ApiKey = "sk_test_51SSAQMBOxzp7UNACemUl8X68qvF8bywcPV87XnqZ9ssbQfcHQaQpquwNslptpjWCd6fOsU726IwIGsUVdFP0axBg00kpqo6W1l";

            var service = new SessionService();
            var session = service.Get(session_id);

            var orderId = session.ClientReferenceId;

            Guid oid = Guid.Parse(orderId);

            var order = await _context.Orders
                            .Include(x => x.OrderPayments)
                            .FirstOrDefaultAsync(x => x.OrderId == oid);

            if (order == null)
                return BadRequest("Order not found");

            // ✅ Update order
            order.PaymentStatus = "Paid";
            order.OrderStatus = "Confirmed";

            var payment = order.OrderPayments.FirstOrDefault();
            if (payment != null)
            {
                payment.Status = "Success";
                payment.ProviderPaymentId = session.PaymentIntentId;
                payment.ResponseRaw = session.ToJson();
            }

            await _context.SaveChangesAsync();

            return Ok("Order payment confirmed");
        }

        // 🧾 GET: Get All Orders by Logged-in User
        [HttpPost("Create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var userIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User ID missing in token.");

            Guid userId = Guid.Parse(userIdClaim);

            // 🛒 Fetch Cart Item
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.CartItemID == request.CartItemId && c.Appuserid == userId);

            if (cartItem == null)
                return BadRequest("Invalid or missing cart item.");

            // 🧾 Calculate totals
            decimal subTotal = cartItem.Price * cartItem.Quantity;
            decimal total = subTotal + request.ShippingAmount + request.TaxAmount;

            // 🧾 Create Main Order
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{new Random().Next(10000, 99999)}",
                AppUserId = userId,           // Buyer
                UsersId = cartItem.usersid,   // Seller/Admin
                ProductId = cartItem.ProductId,
                CartItemId = cartItem.CartItemID,
                SubTotalAmount = subTotal,
                ShippingAmount = request.ShippingAmount,
                TaxAmount = request.TaxAmount,
                TotalAmount = total,
                PaymentMethod = request.PaymentMethod ?? "COD",
                PaymentStatus = "---",
                OrderStatus = "New",
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
            };

            // 🧩 Add Order Item (from Cart)
            order.OrderItems = new List<OrderItem>
        {
            new OrderItem
            {
                OrderId = order.OrderId,
                ProductId = cartItem.ProductId,
                UsersId = cartItem.usersid,
                AppUserId = userId,
                ProductName = cartItem.Name,
                Description = cartItem.Description,
                ImageUrl = cartItem.ImageUrl,
                UnitPrice = cartItem.Price,
                Quantity = cartItem.Quantity
            }
        };

            // 🏠 Add Address (from UI)
            order.OrderAddresses = new List<OrderAddress>
        {
            new OrderAddress
            {
                OrderId = order.OrderId,
                FullName = request.FullName,
                Line1 = request.Line1,
                Line2 = request.Line2,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country,
                Phone = request.Phone
            }
        };

            // 💳 Add Payment Info
            order.OrderPayments = new List<OrderPayment>
        {
            new OrderPayment
            {
                OrderId = order.OrderId,
                PaymentProvider = request.PaymentMethod ?? "COD",
                Amount = total,
                Status = "Pending"
            }
        };

            // 📜 Add Status History
            order.OrderStatusHistory = new List<OrderStatusHistory>
        {
            new OrderStatusHistory
            {
                OrderId = order.OrderId,
                FromStatus = "New",
                ToStatus = "Pending",
                Note = "Ordersuccessfully"
            }
        };


            // 💾 Save All
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 🗑️ Optional: Remove item from cart
            //   _context.CartItems.Remove(cartItem);
            // await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Order placed successfully!",
                orderId = order.OrderId,
                total = order.TotalAmount
            });
        }




        //[Authorize]
        //[HttpPost("Create")]
        //public async Task<IActionResult> CreateOrder([FromBody] Order order)
        //{
        //    var userIdClaim = User.FindFirstValue("UserId");
        //    if (string.IsNullOrEmpty(userIdClaim))
        //        return Unauthorized("User ID missing in token.");

        //    Guid userId = Guid.Parse(userIdClaim);

        //    // ✅ Always create new OrderId before adding to context
        //    order.OrderId = Guid.NewGuid();
        //    order.AppUserId = userId;
        //    order.OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{new Random().Next(10000, 99999)}";
        //    order.CreatedAt = DateTime.Now;

        //    // ✅ Fix child relationships
        //    if (order.OrderItems != null)
        //    {
        //        foreach (var item in order.OrderItems)
        //        {
        //            item.OrderId = order.OrderId;
        //            item.AppUserId = order.AppUserId;
        //        }
        //    }

        //    if (order.OrderAddresses != null)
        //    {
        //        foreach (var address in order.OrderAddresses)
        //        {
        //            address.OrderId = order.OrderId;
        //        }
        //    }

        //    if (order.OrderPayments != null)
        //    {
        //        foreach (var pay in order.OrderPayments)
        //        {
        //            pay.OrderId = order.OrderId;
        //        }
        //    }

        //    if (order.OrderStatusHistory != null)
        //    {
        //        foreach (var history in order.OrderStatusHistory)
        //        {
        //            history.OrderId = order.OrderId;
        //        }
        //    }

        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "Order placed successfully!", orderId = order.OrderId });
        //}

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




        //// 📦 GET: Order by Id
        //[HttpGet("{orderId}")]
        //public async Task<IActionResult> GetOrderById(Guid orderId)
        //{
        //    var order = await _context.Orders
        //        .Include(o => o.OrderItems)
        //        .Include(o => o.OrderAddresses)
        //        .Include(o => o.OrderPayments)
        //        .FirstOrDefaultAsync(o => o.OrderId == orderId);

        //    if (order == null)
        //        return NotFound("Order not found");

        //    return Ok(order);
        //}

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

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };

            return new JsonResult(order, jsonOptions);
        }

        [HttpGet("GetByUserr")]
        public async Task<IActionResult> GetOrdersByUser()
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (userId == null)
                return Unauthorized("User not logged in");

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.AppUserId == Guid.Parse(userId) && o.IsDeleted == false)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders);
        }
        //[HttpGet("ByUser/{userId}")]
        //public async Task<IActionResult> GetOrdersByUser(Guid appUserId)
        //{
        //    var orders = await _context.Orders
        //        .Include(o => o.OrderItems)
        //        .Include(o => o.OrderAddresses)
        //        .Include(o => o.OrderPayments)
        //        .Where(o => o.AppUserId == appUserId && o.IsDeleted == false)
        //        .OrderByDescending(o => o.CreatedAt)
        //        .ToListAsync();

        //    if (orders == null || orders.Count == 0)
        //        return NotFound("No orders found for this user.");

        //    var jsonOptions = new JsonSerializerOptions
        //    {
        //        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        //        WriteIndented = true
        //    };

        //    return new JsonResult(orders, jsonOptions);
        //}
        //    [HttpGet("ByUser/{userId}")]
        //    public async Task<IActionResult> GetOrdersByUser(Guid userId)
        //    {
        //        var orders = await _context.Orders
        //            .Include(o => o.OrderItems)
        //            .Include(o => o.OrderAddresses)
        //            .Include(o => o.OrderPayments)
        //            .Where(o => o.AppUserId == userId && o.IsDeleted == false)
        //            .OrderByDescending(o => o.CreatedAt)
        //            .ToListAsync();

        //        if (orders == null || orders.Count == 0)
        //            return NotFound("No orders found for this user.");

        //        var jsonOptions = new JsonSerializerOptions
        //        {
        //            ReferenceHandler = ReferenceHandler.IgnoreCycles,
        //            WriteIndented = true
        //        };

        //        return new JsonResult(orders, jsonOptions);
        //    }

        //}
        // ✅ GET: api/Orders/ByUser/{userId}
        [HttpGet("ByUser/{userId}")]
        public async Task<IActionResult> GetOrdersByUser(Guid userId)
        {
            try
            {
                var orders = await _context.Orders
                    .Where(o => o.AppUserId == userId && o.IsDeleted == false)
                    .Include(o => o.OrderItems)
                    .Include(o => o.OrderAddresses)
                    .Include(o => o.OrderPayments)
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(o => new
                    {
                        o.OrderId,
                        o.OrderNumber,
                        o.TotalAmount,
                        o.PaymentMethod,
                        o.PaymentStatus,
                        o.OrderStatus,
                        o.CreatedAt,
                        OrderItems = o.OrderItems.Select(i => new
                        {
                            i.ProductName,
                            i.ImageUrl,
                            i.UnitPrice,
                            i.Quantity
                        })
                    })
                    .ToListAsync();

                if (orders == null || !orders.Any())
                    return Ok(new List<object>());

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Error fetching orders: {ex.Message}");
            }
        }

    }
}
