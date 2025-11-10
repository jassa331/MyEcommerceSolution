namespace User.API.Models
{
    public class UsersProfile
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }
    }
}
