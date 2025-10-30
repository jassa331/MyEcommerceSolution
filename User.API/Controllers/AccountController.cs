using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using User.API.DAL;
using User.API.Models;

namespace User.API.Controllers // or Admin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly userportaldbcontext _context;

        public AccountController(IConfiguration configuration, userportaldbcontext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register(UserLogin model)
        {
            model.Password = EncryptionHelper.Encrypt(model.Password);
            _context.Usersofuserportal.Add(model);
            _context.SaveChanges();
            return Ok("User registered successfully!");
        }

        //[HttpPost("login")]
        //public IActionResult Login(UserLogin model)
        //{
        //    var user = _context.Usersofuserportal.FirstOrDefault(x => x.Email == model.Email);
        //    if (user == null) return Unauthorized("Invalid Email");

        //    var decryptedPassword = EncryptionHelper.Decrypt(user.Password);
        //    if (model.Password != decryptedPassword) return Unauthorized("Invalid Password");

        //    var token = GenerateJwtToken(user.Email);
        //    return Ok(new { Token = token });
        //}
        [HttpPost("login")]
        public IActionResult Login(UserLogin model)
        {
            var user = _context.Usersofuserportal.FirstOrDefault(x => x.Email == model.Email);
            if (user == null) return Unauthorized("Invalid Email");

            var decryptedPassword = EncryptionHelper.Decrypt(user.Password);
            if (model.Password != decryptedPassword) return Unauthorized("Invalid Password");

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                Token = token,
                userId = user.Id,
                RedirectUrl = "/user-dashboard"
            });

        }
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
                return BadRequest("No token found.");

            _context.RevokedTokens.Add(new RevokedToken
            {
                Token = token,
                ExpiryDate = DateTime.UtcNow.AddHours(1)
            });

            _context.SaveChanges();
            return Ok("User logged out successfully!");
        }


        private string GenerateJwtToken(UserLogin user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
{
    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // only GUID here
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
};


            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
