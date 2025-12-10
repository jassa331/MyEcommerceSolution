namespace Admin.API.Models
{
    public class ApiLog
    {
        public int Id { get; set; }
        public Guid? UserId { get; set; }
        public string IpAddress { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public int StatusCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
