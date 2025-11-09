using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Authentication.Models
{
    public class Userr
    {
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsAdmin { get; set; } = false;

        public string? UserName { get; set; }   // Nullable
        public string? MobileNumber { get; set; } // Nullable
        //public string? ResetToken { get; set; }
        //public DateTime? TokenExpiry { get; set; }
    }

    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [JsonPropertyName("confirmPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be 10 digits.")]
        [JsonPropertyName("mobileNumber")]
        public string MobileNumber { get; set; } = string.Empty;

        [JsonPropertyName("isAdmin")]
        public bool IsAdmin { get; set; } = false;
    }
    //public class ForgotPasswordDto
    //{

    //    [Required(ErrorMessage = "Email is required.")]
    //    [EmailAddress(ErrorMessage = "Invalid email format.")]
    //    [JsonPropertyName("email")]
    //    public string Email { get; set; } = string.Empty;

    //}

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
