using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajPortal.Data;

namespace StajPortal.Controllers
{
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Jobs
        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? city, string? department)
        {
            var jobsQuery = _context.JobPostings
                .Include(j => j.Company)
                .Where(j => j.IsActive && j.IsApproved)
                .AsQueryable();

            // Arama filtreleri
            if (!string.IsNullOrWhiteSpace(search))
            {
                jobsQuery = jobsQuery.Where(j => 
                    j.Title.Contains(search) || 
                    j.Description.Contains(search) ||
                    j.Company.CompanyName.Contains(search));
                ViewBag.SearchTerm = search;
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                jobsQuery = jobsQuery.Where(j => j.City != null && j.City.Contains(city));
                ViewBag.City = city;
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                jobsQuery = jobsQuery.Where(j => j.Department != null && j.Department.Contains(department));
                ViewBag.Department = department;
            }

            var jobs = await jobsQuery
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            ViewBag.TotalJobs = jobs.Count;

            return View(jobs);
        }

        // GET: /Jobs/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.JobPostings
                .Include(j => j.Company)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
            {
                TempData["Error"] = "İlan bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (!job.IsActive || !job.IsApproved)
            {
                TempData["Error"] = "Bu ilan artık aktif değil.";
                return RedirectToAction(nameof(Index));
            }

            // Kullanıcı daha önce başvurmuş mu kontrol et
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
                if (user?.Role == "Student")
                {
                    var studentProfile = await _context.StudentProfiles
                        .FirstOrDefaultAsync(s => s.UserId == user.Id);
                    
                    if (studentProfile != null)
                    {
                        var hasApplied = await _context.Applications
                            .AnyAsync(a => a.JobPostingId == id && a.StudentId == studentProfile.Id);
                        ViewBag.HasApplied = hasApplied;
                    }
                }
            }

            return View(job);
        }
    }
}

