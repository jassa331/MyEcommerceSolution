using System.ComponentModel.DataAnnotations;

namespace Authentication.Models
{
    public class Userr
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;
    }

   

public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }

        public bool IsAdmin { get; set; } = false;
    }


    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
