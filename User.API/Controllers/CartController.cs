using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using User.API.DAL;
using User.API.Models;

namespace User.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly userportaldbcontext _context;

        public CartController(userportaldbcontext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("Add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemDto cartItem)
        {
            if (cartItem == null)
                return BadRequest("Invalid data");

            // ✅ Extract logged-in user ID (buyer) from JWT token
            var guidClaim = User.Claims.FirstOrDefault(c => Guid.TryParse(c.Value, out _))?.Value;
            if (string.IsNullOrEmpty(guidClaim))
                return BadRequest("No valid User ID found in token");

            Guid buyerId = Guid.Parse(guidClaim);

            // ✅ Get product info (and which admin uploaded it)
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cartItem.ProductId);
            if (product == null)
                return NotFound("Product not found");

            // ✅ Check if already exists in cart (to update quantity)
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UsersofuserportalID == buyerId && c.ProductId == product.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += cartItem.Quantity;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Cart updated successfully!" });
            }

            // ✅ Create new cart item
            var newItem = new CartItem
            {
                CartItemID = Guid.NewGuid(),
                UsersofuserportalID = buyerId,  // 👈 logged-in user
                usersid = product.usersid,      // 👈 admin (from product)
                ProductId = product.ProductId,
                ProductName = product.Name,
                ProductDescription = product.Description,
                ProductPrice = product.Price,
                Quantity = cartItem.Quantity,
                ProductImageUrl = product.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.CartItems.Add(newItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Item added to cart!" });
        }

        // ✅ Get cart items for currently logged-in user

        [Authorize]
        [HttpGet("show-token")]
        public IActionResult ShowTokenClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }

        [Authorize]
        [HttpGet("GetByUser")]
        public async Task<IActionResult> GetCartItems()
        {
            // ✅ Get GUID claim from JWT
            var guidClaim = User.Claims
                                .FirstOrDefault(c => Guid.TryParse(c.Value, out _))?.Value;

            if (string.IsNullOrEmpty(guidClaim))
                return BadRequest("No valid User ID found in token");

            Guid usersofuserportalID = Guid.Parse(guidClaim);

            // ✅ Filter cart items by logged-in user
            var cartItems = await _context.CartItems
                .Where(c => c.UsersofuserportalID == usersofuserportalID)
                .Select(c => new
                {
                    c.CartItemID,
                    c.ProductName,
                    c.ProductDescription,
                    c.ProductPrice,
                    c.Quantity,
                    c.ProductImageUrl
                })
                .ToListAsync();

            if (!cartItems.Any())
                return NotFound("No cart items found for this user.");

            return Ok(cartItems);
        }

    }
}
