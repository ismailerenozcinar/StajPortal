using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StajPortal.Models.Entities
{
    public class StudentProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? University { get; set; }

        [StringLength(200)]
        public string? Department { get; set; }

        public int? GraduationYear { get; set; }

        public decimal? GPA { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(500)]
        public string? CVFilePath { get; set; }

        [StringLength(500)]
        public string? CVLink { get; set; }

        [StringLength(1000)]
        public string? About { get; set; }

        [StringLength(500)]
        public string? ProfileImagePath { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}

