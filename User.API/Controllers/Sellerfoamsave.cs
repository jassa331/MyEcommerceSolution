using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using User.API.DAL;
using User.API.Models;

namespace User.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class Sellerfoamsave : ControllerBase
    {
        public readonly userportaldbcontext _seller;
        public Sellerfoamsave(userportaldbcontext sellerinfo)
        {
            _seller = sellerinfo;
        }
        [HttpPost("save-seller-foam")]
        public async Task<IActionResult> addnewsellerfoam([FromBody] SELLERFOAM SELLER)
        {
            var userId = User.FindFirst("UserId")?.Value ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in or claim missing");

            if (SELLER == null)
            {
                return BadRequest("Invalid seller foam data");
            }
            try
            {
                _seller.seller.Add(SELLER);
                await _seller.SaveChangesAsync();
                return Ok("Seller created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong: " + ex.Message);
            }


        }



    }

}