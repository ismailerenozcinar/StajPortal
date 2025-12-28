using Microsoft.AspNetCore.Identity;

namespace StajPortal.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string Role { get; set; } = "Student"; // Student, Company, Admin
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public StudentProfile? StudentProfile { get; set; }
        public CompanyProfile? CompanyProfile { get; set; }
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    }
}

