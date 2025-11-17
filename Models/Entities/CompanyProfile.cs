using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StajPortal.Models.Entities
{
    public class CompanyProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? CompanyLogoPath { get; set; }

        [StringLength(100)]
        public string? Industry { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }

        [StringLength(2000)]
        public string? About { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;
        public ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
    }
}

