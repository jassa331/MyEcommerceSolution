using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User.API.DAL;
using User.API.Models;
//[Authorize(Roles = "User,Admin")] // Only users with role "User" can access
[Route("api/[controller]")]
[ApiController]
public class ProductImageController : ControllerBase
{
    private readonly userportaldbcontext _context;
    private readonly IWebHostEnvironment _env;

    public ProductImageController(userportaldbcontext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    //[HttpPost("upload")]
    //[Consumes("multipart/form-data")]
 
    //public async Task<IActionResult> Upload([FromForm] ProductUploadDto model)
    //{
    //    if (model.File == null) return BadRequest("No file uploaded");

    //    // Fallback if WebRootPath is null
    //    var webRoot = _env.WebRootPath;
    //    if (string.IsNullOrEmpty(webRoot))
    //    {
    //        webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    //    }

    //    // Ensure images folder exists
    //    var imagesFolder = Path.Combine(webRoot, "images");
    //    if (!Directory.Exists(imagesFolder))
    //        Directory.CreateDirectory(imagesFolder);

    //    // Save file
    //    var filePath = Path.Combine(imagesFolder, model.File.FileName);
    //    using (var stream = new FileStream(filePath, FileMode.Create))
    //    {
    //        await model.File.CopyToAsync(stream);
    //    }

    //    // Create accessible URL
    //    var baseUrl = $"{Request.Scheme}://{Request.Host}";
    //    var imageUrl = $"{baseUrl}/images/{model.File.FileName}";

    //    // Save product in DB
    //    var product = new Product
    //    {
    //        Name = model.ProductName,
    //        Description = model.Description,
    //        Price = model.Price,
    //        ImageUrl = imageUrl
    //    };

    //    _context.Productss.Add(product);
    //    await _context.SaveChangesAsync();

    //    return Ok(product);
    //}


    [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products.ToListAsync();
        return Ok(products);
    }
}
