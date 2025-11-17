namespace StajPortal.Models.DTOs
{
    public class JobPostingDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? City { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public CompanyDto? Company { get; set; }
    }

    public class CompanyDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Industry { get; set; }
        public string? City { get; set; }
        public string? Website { get; set; }
    }

    public class CreateJobPostingDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? City { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

