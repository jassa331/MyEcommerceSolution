using Admin.API.DAL;
using Admin.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Linq;

namespace Admin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiLog]
    public class Manage_orders : ControllerBase
    {
        public readonly admindbcontext _orders;
        public Manage_orders(admindbcontext orders)
        {
            _orders = orders;
        }
        [Authorize]
        [HttpGet("getallorders")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized("Invalid credentials");

                // Step 1: fetch all orders for this user
                var orders = await _orders.Orders
                    .Where(o => o.Usersid == userId && o.IsDeleted == false)
                    .ToListAsync();

                if (!orders.Any())
                    return Ok(new List<OrderFullDto>()); // empty list if no orders

                var orderIds = orders.Select(o => o.OrderId).ToList();

                // Step 2: fetch items and addresses for these orders
                var items = await _orders.OrderItems
                    .Where(i => i.UsersId == userId && orderIds.Contains(i.OrderId))
                    .ToListAsync();

                var addresses = await _orders.OrderAddresses
                    .Where(a => orderIds.Contains(a.OrderId))
                    .ToListAsync();

                // Step 3: build final response
                var result = orders.Select(o => new OrderFullDto
                {
                    Order = o,
                    Items = items.Where(i => i.OrderId == o.OrderId).ToList(),
                    Addresses = addresses.Where(a => a.OrderId == o.OrderId).ToList()
                }).ToList();

                return Ok(result);
            }
            catch
            {
                return StatusCode(500, "Something went wrong.");
            }
        }
        [HttpPut("update-order-status/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromBody] UpdateStatusDto dto)
        {
            try
            {
                var order = await _orders.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);

                if (order == null)
                    return NotFound("Order not found");

                var validStatus = new List<string> { "Pending", "Shipped", "Delivered", "Cancelled" };

                if (!validStatus.Contains(dto.OrderStatus))
                    return BadRequest("Invalid status");

                // Update fields
                order.OrderStatus = dto.OrderStatus;
                order.UpdatedAt = DateTime.UtcNow;

                if (dto.OrderStatus == "Delivered")
                    order.PaymentStatus = "Paid";

                if (dto.OrderStatus == "Cancelled")
                    order.IsActive = false;

                await _orders.SaveChangesAsync();

                return Ok(new { Message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong");
            }
        }



        [HttpGet("download-invoice/{orderId}")]
        public async Task<IActionResult> DownloadInvoice(Guid orderId)
        {
            var order = await _orders.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return NotFound();

            var items = await _orders.OrderItems
                .Where(i => i.OrderId == orderId)
                .ToListAsync();

            var address = await _orders.OrderAddresses
                .FirstOrDefaultAsync(a => a.OrderId == orderId);

            var doc = new InvoiceDocument(order, items, address);
            byte[] pdfBytes = doc.GeneratePdf();

            return File(pdfBytes, "application/pdf", $"Invoice-{order.OrderNumber}.pdf");
        }

    }
}
