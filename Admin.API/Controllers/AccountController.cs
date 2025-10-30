using Admin.API.DAL;
using Admin.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Admin.API.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly admindbcontext _context;

        public AccountController(IConfiguration configuration, admindbcontext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // ✅ REGISTER USER
        [HttpPost("registert")]
        public IActionResult Register(AdminLogin model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Email and password are required.");

            model.Email = model.Email.Trim().ToLower();
            model.Password = EncryptionHelper.Encrypt(model.Password);

            _context.Users.Add(model);
            _context.SaveChanges();

            return Ok("User registered successfully!");
        }

        // ✅ LOGIN USER
        [HttpPost("logins")]
        public IActionResult Login([FromBody] AdminLogin model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email))
                return BadRequest("Email is required.");

            var email = model.Email.Trim().ToLower();

            var user = _context.Users.FirstOrDefault(x => x.Email.ToLower() == email);
            if (user == null)
                return Unauthorized("User not found.");

            var decryptedPassword = EncryptionHelper.Decrypt(user.Password);
            if (model.Password != decryptedPassword)
                return Unauthorized("Invalid Password");

            // ✅ Generate JWT token including UserId claim
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email
            });
        }

        // ✅ TOKEN GENERATOR
        private string GenerateJwtToken(AdminLogin user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // 👈 very important for Blazor
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
