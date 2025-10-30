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
            if (await _context.Userr.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists.");

            var user = new Userr
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                IsAdmin = dto.IsAdmin
            };

            _context.Userr.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Registered successfully.");
        }

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
                RedirectUrl = user.IsAdmin ? "https://localhost:7066/" : "https://localhost:7117/user-dashboard"
            });
        }

        // ✅ FIXED GenerateJwtToken method
        private string GenerateJwtToken(Userr user)
        {
            var claims = new List<Claim>
    {
        new Claim("UserId", user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim("IsAdmin", user.IsAdmin ? "True" : "False"),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

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
