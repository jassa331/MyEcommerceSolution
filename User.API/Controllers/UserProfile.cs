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
    public class UserProfileController : ControllerBase
    {
        private readonly userportaldbcontext _profile;

        // ✅ Inject your DbContext here
        public UserProfileController(userportaldbcontext profile)
        {
            _profile = profile;
        }

        [HttpGet("get-user-profile")]
        public IActionResult GetUserProfile()
        {
            var userId = User.FindFirst("UserId")?.Value ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in or claim missing");

            Guid profileId = Guid.Parse(userId);

            var profile = _profile.userr
                .Where(g => g.Id == profileId)
                .Select(g => new
                {
                    g.Id,
                    g.UserName,
                    g.Email,
                    g.MobileNumber,
                    g.IsAdmin
                })
                .FirstOrDefault();

            if (profile == null)
                return NotFound("Profile not found");

            return Ok(profile);
        }
    }
}






