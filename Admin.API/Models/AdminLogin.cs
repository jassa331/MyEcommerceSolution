using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Admin.API.Models
{
    public class AdminLogin
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }

}
