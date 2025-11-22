using Authentication.DAL;
using Authentication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _config;

        public AccountController(AuthDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                // ✅ 1. Basic field validation
                if (string.IsNullOrWhiteSpace(dto.Email) ||
                    string.IsNullOrWhiteSpace(dto.Password) ||
                    string.IsNullOrWhiteSpace(dto.ConfirmPassword))
                {
                    return BadRequest("Please fill all required fields.");
                }

                // ✅ 2. Password confirmation check
                if (dto.Password != dto.ConfirmPassword)
                {
                    return BadRequest("Password and Confirm Password do not match.");
                }

                // ✅ 3. Email uniqueness check
                if (await _context.Userr.AnyAsync(u => u.Email == dto.Email))
                {
                    return BadRequest("This email is already registered. Please use another one.");
                }

                // ✅ 4. Create and save user
                var user = new Userr
                {
                    Email = dto.Email.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    IsAdmin = dto.IsAdmin,
                    UserName = dto.UserName?.Trim(),
                    MobileNumber = dto.MobileNumber?.Trim()
                };

                _context.Userr.Add(user);
                await _context.SaveChangesAsync();

                return Ok("Registered successfully!");
            }
            catch (DbUpdateException dbEx)
            {
                // SQL or DB related errors
                return StatusCode(500, "A database error occurred. Please enter valid data or try again.");
            }
            catch (Exception ex)
            {
                // General unexpected errors
                return StatusCode(500, $"Something went wrong. Please enter valid data. Error: {ex.Message}");
            }
        }


        //[HttpPost("register")]
        //public async Task<IActionResult> Register(RegisterDto dto)
        //{
        //    try
        //    {
        //        if (await _context.Userr.AnyAsync(u => u.Email == dto.Email))
        //            return BadRequest("Email already exists.");

        //        var user = new Userr
        //        {
        //            Email = dto.Email,
        //            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        //            IsAdmin = dto.IsAdmin
        //        };

        //        _context.Userr.Add(user);
        //        await _context.SaveChangesAsync();

        //    return Ok("Registered successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Return 500 with the exception message
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}
        //[HttpPost("reset-password")]
        //public async Task<IActionResult> ResetPassword(string email, string token, string newPassword)
        //{
        //    var user = await _context.Userr.FirstOrDefaultAsync(u => u.Email == email && u.ResetToken == token);

        //    if (user == null || user.TokenExpiry < DateTime.UtcNow)
        //        return BadRequest("Invalid or expired reset token.");

        //    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        //    user.ResetToken = null;
        //    user.TokenExpiry = null;

        //    await _context.SaveChangesAsync();
        //    return Ok("Password reset successfully!");
        //}
        //[HttpPost("forgot-password")]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        //{
        //    try
        //    {
        //        // ✅ 1. Validate input
        //        if (string.IsNullOrWhiteSpace(dto.Email))
        //            return BadRequest("Email is required.");

        //        // ✅ 2. Check if user exists
        //        var user = await _context.Userr.FirstOrDefaultAsync(u => u.Email == dto.Email);
        //        if (user == null)
        //            return NotFound("No account found with this email.");

        //        // ✅ 3. Generate a reset token (random GUID for simplicity)
        //        var resetToken = Guid.NewGuid().ToString();

        //        // ✅ 4. Save it to user table (add a new column if not exists)
        //        user.ResetToken = resetToken;
        //        user.TokenExpiry = DateTime.UtcNow.AddMinutes(15);
        //        await _context.SaveChangesAsync();

        //        // ✅ 5. Normally, you'd send email — for now, just return token
        //        return Ok($"Your reset token is: {resetToken}");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Something went wrong: {ex.Message}");
        //    }
        //}



        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Userr.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            var token = GenerateJwtToken(user);
            Console.WriteLine($"✅ TOKEN GENERATED FOR {user.Email}: {token}");


            Console.WriteLine($"JWT SETTINGS -> Key: {_config["Jwt:Key"]}, Issuer: {_config["Jwt:Issuer"]}, Audience: {_config["Jwt:Audience"]}");


            return Ok(new
            {
                Token = token,
                UserId = user.Id,
                IsAdmin = user.IsAdmin,
                RedirectUrl = user.IsAdmin ? "https://localhost:7066/" : "http://localhost:5231//user-dashboard"
            });
        }

        // ✅ FIXED GenerateJwtToken method
        private string GenerateJwtToken(Userr user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ✅ important
            new Claim("UserId", user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim("IsAdmin", user.IsAdmin ? "True" : "False"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            //        var claims = new List<Claim>
            //{
            //    new Claim("UserId", user.Id.ToString()),
            //    new Claim(ClaimTypes.Email, user.Email),
            //    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            //    new Claim("IsAdmin", user.IsAdmin ? "True" : "False"),
            //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            //};

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
