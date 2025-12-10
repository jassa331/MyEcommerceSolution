using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User.API.DAL;

namespace User.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        public readonly userportaldbcontext _geteletronics;
        public UserController(userportaldbcontext geteletronics)
        {
            _geteletronics = geteletronics;
        }
        [HttpGet("dashboard")]
        public async Task<IActionResult> getall()
        {
            var userId = User.FindFirst("UserId")?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in or claim missing");

            var products = await _geteletronics.Products
                .Where(p => p.Category == "Electronics" && !p.IsDeleted)
                .ToListAsync();

            return Ok(products);
        }
        [HttpGet("fashion-products")]
        public async Task<IActionResult> getfashion()
        {
            var userId = User.FindFirst("UserId")?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in or claim missing");

            var products = await _geteletronics.Products
                .Where(p => p.Category == "Fashion" && !p.IsDeleted)
                .ToListAsync();

            return Ok(products);
        }
        [HttpGet("sports-products")]
        public async Task<IActionResult> getsports()
        {
            var userId = User.FindFirst("UserId")?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in or claim missing");

            var products = await _geteletronics.Products
                .Where(p => p.Category == "Sports" && !p.IsDeleted)
                .ToListAsync();

            return Ok(products);
        }
        [HttpGet("HomeAppliances-products")]
        public async Task<IActionResult> gethomeappliances()
        {
            var userId = User.FindFirst("UserId")?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in or claim missing");

            var products = await _geteletronics.Products
                .Where(p => p.Category == "Home Appliances" && !p.IsDeleted)
                .ToListAsync();

            return Ok(products);
        }
        [HttpGet("Books&Stationery")]
        public async Task<IActionResult> getBooksStationery()
        {
            var userId = User.FindFirst("UserId")?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in or claim missing");

            var products = await _geteletronics.Products
                .Where(p => p.Category == "Books & Stationery" && !p.IsDeleted)
                .ToListAsync();

            return Ok(products);
        }
        [HttpGet("Beauty&PersonalCare")]
        public async Task<IActionResult> getBeautyPersonalCare()
        {
            var userId = User.FindFirst("UserId")?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in or claim missing");

            var products = await _geteletronics.Products
                 .Where(p => p.Category == "Beauty & Personal Care" && !p.IsDeleted)
                 .ToListAsync();

            return Ok(products);
        }
    }

    //that is MemberAccessException 
}