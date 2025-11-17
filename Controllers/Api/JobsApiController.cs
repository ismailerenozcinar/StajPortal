using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajPortal.Data;
using StajPortal.Models.DTOs;
using StajPortal.Models.Entities;

namespace StajPortal.Controllers.Api
{

    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class JobsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobsApiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<JobPostingDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<JobPostingDto>>> GetJobs(
            [FromQuery] string? search,
            [FromQuery] string? city,
            [FromQuery] string? department)
        {
            var query = _context.JobPostings
                .Include(j => j.Company)
                .Where(j => j.IsActive && j.IsApproved)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(j =>
                    j.Title.Contains(search) ||
                    j.Description.Contains(search) ||
                    j.Company.CompanyName.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(j => j.City != null && j.City.Contains(city));
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(j => j.Department != null && j.Department.Contains(department));
            }

            var jobs = await query
                .OrderByDescending(j => j.CreatedAt)
                .Select(j => new JobPostingDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Department = j.Department,
                    City = j.City,
                    StartDate = j.StartDate,
                    EndDate = j.EndDate,
                    IsActive = j.IsActive,
                    IsApproved = j.IsApproved,
                    CreatedAt = j.CreatedAt,
                    Company = new CompanyDto
                    {
                        Id = j.Company.Id,
                        CompanyName = j.Company.CompanyName,
                        Industry = j.Company.Industry,
                        City = j.Company.City,
                        Website = j.Company.Website
                    }
                })
                .ToListAsync();

            return Ok(jobs);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JobPostingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<JobPostingDto>> GetJob(int id)
        {
            var job = await _context.JobPostings
                .Include(j => j.Company)
                .Where(j => j.Id == id && j.IsActive && j.IsApproved)
                .Select(j => new JobPostingDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Department = j.Department,
                    City = j.City,
                    StartDate = j.StartDate,
                    EndDate = j.EndDate,
                    IsActive = j.IsActive,
                    IsApproved = j.IsApproved,
                    CreatedAt = j.CreatedAt,
                    Company = new CompanyDto
                    {
                        Id = j.Company.Id,
                        CompanyName = j.Company.CompanyName,
                        Industry = j.Company.Industry,
                        City = j.Company.City,
                        Website = j.Company.Website
                    }
                })
                .FirstOrDefaultAsync();

            if (job == null)
            {
                return NotFound(new { message = "İlan bulunamadı" });
            }

            return Ok(job);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<JobPostingDto>> CreateJob([FromBody] CreateJobPostingDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Giriş yapmanız gerekiyor" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user?.Role != "Company")
            {
                return Forbid();
            }

            var company = await _context.CompanyProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (company == null)
            {
                return BadRequest(new { message = "Firma profili bulunamadı" });
            }

            var job = new JobPosting
            {
                CompanyId = company.Id,
                Title = dto.Title,
                Description = dto.Description,
                Department = dto.Department,
                City = dto.City,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true,
                IsApproved = false, // Admin onayı bekliyor
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.JobPostings.Add(job);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, new
            {
                success = true,
                message = "İlan oluşturuldu, admin onayı bekleniyor",
                jobId = job.Id
            });
        }
        [Authorize]
        [HttpPost("{id}/apply")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApplyToJob(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Giriş yapmanız gerekiyor" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user?.Role != "Student")
            {
                return BadRequest(new { message = "Sadece öğrenciler başvuru yapabilir" });
            }

            var student = await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return BadRequest(new { message = "Öğrenci profili bulunamadı" });
            }

            var job = await _context.JobPostings
                .FirstOrDefaultAsync(j => j.Id == id && j.IsActive && j.IsApproved);

            if (job == null)
            {
                return NotFound(new { message = "İlan bulunamadı" });
            }

            // Daha önce başvurmuş mu kontrol et
            var existingApplication = await _context.Applications
                .AnyAsync(a => a.JobPostingId == id && a.StudentId == student.Id);

            if (existingApplication)
            {
                return BadRequest(new { message = "Bu ilana zaten başvurdunuz" });
            }

            var application = new Models.Entities.Application
            {
                JobPostingId = id,
                StudentId = student.Id,
                Status = "Pending",
                AppliedAt = DateTime.UtcNow
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Başvuru başarıyla gönderildi",
                applicationId = application.Id
            });
        }
    }
}

