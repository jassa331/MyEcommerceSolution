using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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


        [HttpPost("Add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemDto cartItem)
        {
            if (cartItem == null)
                return BadRequest("Invalid data");

            // 1️⃣ Get buyer ID from JWT
            var userIdClaim = User.Claims.FirstOrDefault(c => Guid.TryParse(c.Value, out _))?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return BadRequest("No valid user ID found in token.");

            Guid buyerId = Guid.Parse(userIdClaim);

            // 2️⃣ Fetch product
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cartItem.ProductId);
            if (product == null)
                return NotFound("Product not found.");

            // 3️⃣ Check if product already exists in active cart
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Appuserid == buyerId && c.ProductId == product.ProductId);

            if (existingItem != null)
            {
                if (existingItem.IsDeleted)
                {
                    // 🟢 Reactivate deleted cart item
                    existingItem.IsDeleted = false;
                    existingItem.Quantity = cartItem.Quantity; // reset or update as needed
                    existingItem.CreatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "♻️ Product re-added to cart!" });
                }

                // 🟡 Item already in cart → increase quantity
                existingItem.Quantity += cartItem.Quantity;
                await _context.SaveChangesAsync();
                return Ok(new { message = "🛒 Product quantity updated!" });
            }

            // 4️⃣ Create new cart item
            var newCartItem = new CartItem
            {
                CartItemID = Guid.NewGuid(),
                Appuserid = buyerId,          // ✅ Buyer
                usersid = product.usersid,    // ✅ Seller
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = cartItem.Quantity,
                ImageUrl = product.ImageUrl,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.CartItems.Add(newCartItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Product added to cart successfully!" });
        }

        //[HttpPost("Add")]
        //public async Task<IActionResult> AddToCart([FromBody] CartItemDto cartItem)
        //{
        //    if (cartItem == null)
        //        return BadRequest("Invalid data");

        //    // 1️⃣ Get buyer ID from JWT
        //    var userIdClaim = User.Claims.FirstOrDefault(c => Guid.TryParse(c.Value, out _))?.Value;
        //    if (string.IsNullOrEmpty(userIdClaim))
        //        return BadRequest("No valid user ID found in token.");

        //    Guid buyerId = Guid.Parse(userIdClaim);

        //    // 2️⃣ Fetch product
        //    var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cartItem.ProductId);
        //    if (product == null)
        //        return NotFound("Product not found.");

        //    // 3️⃣ Check if product already exists in cart
        //    var existingItem = await _context.CartItems
        //        .FirstOrDefaultAsync(c => c.Appuserid == buyerId && c.ProductId == product.ProductId);

        //    if (existingItem != null)
        //    {
        //        existingItem.Quantity += cartItem.Quantity;
        //        await _context.SaveChangesAsync();
        //        return Ok(new { message = "🛒 Product quantity updated!" });
        //    }

        //    // 4️⃣ Create new cart item
        //    var newCartItem = new CartItem
        //    {
        //        CartItemID = Guid.NewGuid(),
        //        Appuserid = buyerId,          // ✅ Buyer
        //        usersid = product.usersid,    // ✅ Seller
        //        ProductId = product.ProductId,
        //        Name = product.Name,
        //        Description = product.Description,
        //        Price = product.Price,
        //        Quantity = cartItem.Quantity,
        //        ImageUrl = product.ImageUrl,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    _context.CartItems.Add(newCartItem);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "✅ Product added to cart successfully!" });
        //}


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

            // ✅ Filter: only user's active (non-deleted) cart items
            var cartItems = await _context.CartItems
                .Where(c => c.Appuserid == usersofuserportalID && !c.IsDeleted) // 👈 Important line
                .Select(c => new
                {
                    c.CartItemID,
                    c.Name,
                    c.Description,
                    c.Price,
                    c.Quantity,
                    c.ImageUrl
                })
                .ToListAsync();

            if (!cartItems.Any())
                return NotFound("No active cart items found for this user.");

            return Ok(cartItems);
        }

        //[Authorize]
        //[HttpGet("GetByUser")]
        //public async Task<IActionResult> GetCartItems()
        //{
        //    // ✅ Get GUID claim from JWT
        //    var guidClaim = User.Claims
        //                        .FirstOrDefault(c => Guid.TryParse(c.Value, out _))?.Value;

        //    if (string.IsNullOrEmpty(guidClaim))
        //        return BadRequest("No valid User ID found in token");

        //    Guid usersofuserportalID = Guid.Parse(guidClaim);

        //    // ✅ Filter cart items by logged-in user
        //    var cartItems = await _context.CartItems
        //        .Where(c => c.Appuserid == usersofuserportalID)
        //        .Select(c => new
        //        {
        //            c.CartItemID,
        //            c.Name,
        //            c.Description,
        //            c.Price,
        //            c.Quantity,
        //            c.ImageUrl
        //        })
        //        .ToListAsync();

        //    if (!cartItems.Any())
        //        return NotFound("No cart items found for this user.");

        //    return Ok(cartItems);
        //}
        [Authorize]
        [HttpDelete("Delete/{cartItemId}")]
        public async Task<IActionResult> DeleteCartItem(Guid cartItemId)
        {
            // ✅ Get logged-in user ID from JWT
            var userIdClaim = User.Claims.FirstOrDefault(c => Guid.TryParse(c.Value, out _))?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return BadRequest("No valid user ID found in token.");

            Guid buyerId = Guid.Parse(userIdClaim);

            // ✅ Find the cart item (which is not already deleted)
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.CartItemID == cartItemId && c.Appuserid == buyerId && !c.IsDeleted);

            if (cartItem == null)
                return NotFound("Cart item not found or already deleted.");

            // 🚫 Don't physically delete — mark as deleted
            cartItem.IsDeleted = true;
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "🗑️ Cart item removed successfully (soft deleted)." });
        }




    }
}