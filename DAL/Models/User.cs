using System;

namespace DAL.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Auth0Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string Role { get; set; } = "member";
        public DateTime? CreatedAt { get; set; }

        public bool IsAdmin => string.Equals(Role, "admin", StringComparison.OrdinalIgnoreCase);
    }
}
