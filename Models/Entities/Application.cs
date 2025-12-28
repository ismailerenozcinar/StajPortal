using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StajPortal.Models.Entities
{
    public class Application
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JobPostingId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Accepted, Rejected

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedAt { get; set; }

        [StringLength(1000)]
        public string? ReviewNote { get; set; }

        // Navigation Properties
        [ForeignKey("JobPostingId")]
        public JobPosting JobPosting { get; set; } = null!;

        [ForeignKey("StudentId")]
        public StudentProfile Student { get; set; } = null!;

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}

