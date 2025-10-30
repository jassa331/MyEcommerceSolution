using Admin.API.DAL;
using Admin.API.Models;
using Admin.API.NewFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Admin.API.Controllers
{
    [ApiController]
   // [Authorize(Policy = "AdminOnly")]
    [Route("api/[controller]")]
    [Authorize] // Protects all endpoints
   // [Authorize(Policy = "AdminOnly")]
    public class ProductController : ControllerBase
    {
        private readonly admindbcontext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(admindbcontext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ✅ 1. Add Product (Admin only)
         [Authorize]
        // [Authorize(Policy = "AdminOnly")]
       // [AllowAnonymous]
        [HttpPost("add")]
       
        public async Task<IActionResult> AddProduct([FromForm] ProductDto model)
        {
            try
            {
                // ✅ Extract UserId from token
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid parsedUserId))
                    return Unauthorized("Invalid or missing UserId claim in token.");

                string fileUrl = null;

                // ✅ Save image to wwwroot/images
                if (model.Image != null && model.Image.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = $"{Guid.NewGuid()}_{model.Image.FileName}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }

                    fileUrl = $"{Request.Scheme}://{Request.Host}/images/{uniqueFileName}";
                }

                // ✅ Create product entity
                var product = new Product
                {
                    ProductId = Guid.NewGuid(),
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Category = model.Category,
                    ImageUrl = fileUrl,
                    InStock = model.InStock,
                    Usersid = parsedUserId
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"CLAIM: {claim.Type} = {claim.Value}");
                }

                return Ok(new { message = "✅ Product added successfully", product });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Server error: {ex.Message}");
            }
        }

        // ✅ 2. Get All Products (Admin only)
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        // ✅ 3. Get Products of Logged-in User
        [HttpGet("my-products")]
        public async Task<IActionResult> GetMyProducts()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized("Invalid or missing UserId claim.");

            var products = await _context.Products
                .Where(p => p.Usersid == userId)
                .ToListAsync();

            return Ok(products);
        }

        // ✅ 4. Update Product (Owner only)
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductUpdateDto model)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized("Invalid or missing UserId claim.");

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
                return NotFound("Product not found.");

            if (product.Usersid != userId)
                return Forbid("You cannot edit another user's product.");

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Category = model.Category;
            product.InStock = model.InStock;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Product updated successfully", product });
        }

        // ✅ 5. Delete Product (Owner only)
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized("Invalid or missing UserId claim.");

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
                return NotFound("Product not found.");

            if (product.Usersid != userId)
                return Forbid("You cannot delete another user's product.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Product deleted successfully" });
        }
        [AllowAnonymous]
        [HttpGet("show-token")]
        public IActionResult ShowTokenClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }
        // ✅ 6. Get Products by Specific User (Admin only)
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("byuser/{userid}")]
        public async Task<IActionResult> GetProductsByUser(Guid userid)
        {
            var products = await _context.Products
                .Where(p => p.Usersid == userid)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    p.Price,
                    p.Category,
                    p.ImageUrl,
                    p.InStock
                })
                .ToListAsync();

            if (products.Count == 0)
                return NotFound("No products found for this user.");

            return Ok(products);
        }
    }
}
