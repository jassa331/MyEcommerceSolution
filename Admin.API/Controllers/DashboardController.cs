using Admin.API.DAL;
using Admin.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Admin.API.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    [ApiLog]
    public class DashboardController : ControllerBase
    {
        private readonly admindbcontext _context;

        public DashboardController(admindbcontext context)
        {
            _context = context;
        }

        [HttpGet("order-status-chart")]
        public async Task<IActionResult> GetOrderStatusChart()
        {
            // ✅ Extract UserId from token
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid parsedUserId))
                return Unauthorized("Invalid or missing UserId claim in token.");
            var data = await _context.Orders
                .GroupBy(o => o.OrderStatus)
                .Select(g => new OrderStatusChartDto
                {
                    Label = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            // ✅ Extract UserId from token
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid parsedUserId))
                return Unauthorized("Invalid or missing UserId claim in token.");
            var totalActiveProducts = await _context.Products
     .CountAsync(p => (p.IsActive == true && p.IsDeleted == false) && p.Usersid == parsedUserId);

            var totalInactiveProducts = await _context.Products
                .CountAsync(p => (p.IsActive == false || p.IsDeleted == true) && p.Usersid == parsedUserId);


            var onlinePayments = await _context.Orders
                .CountAsync(o => (o.PaymentMethod == "Online") && o.Usersid == parsedUserId);

            var codPayments = await _context.Orders
                .CountAsync(o => (o.PaymentMethod == "COD") && o.Usersid == parsedUserId);

            var result = new DashboardStatsDto
            {
                TotalActiveProducts = totalActiveProducts,
                TotalInactiveProducts = totalInactiveProducts,
                OnlinePayments = onlinePayments,
                CodPayments = codPayments
            };

            return Ok(result);
        }


    }
}