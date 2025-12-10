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
            var totalActiveProducts = await _context.Products
     .CountAsync(p => p.IsActive == true && p.IsDeleted == false);

            var totalInactiveProducts = await _context.Products
                .CountAsync(p => p.IsActive == false || p.IsDeleted == true);


            var onlinePayments = await _context.Orders
                .CountAsync(o => o.PaymentMethod == "Online");

            var codPayments = await _context.Orders
                .CountAsync(o => o.PaymentMethod == "COD");

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