using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajPortal.Data;
using StajPortal.Models.Entities;
using StajPortal.Models.ViewModels;

namespace StajPortal.Controllers
{
    [Authorize(Policy = "CompanyOnly")]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompanyController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var company = await _context.CompanyProfiles
                .FirstOrDefaultAsync(c => c.UserId == user!.Id);

            if (company == null)
            {
                TempData["Error"] = "Firma profili bulunamadı.";
                return RedirectToAction("Profile");
            }

            // İstatistikler
            ViewBag.TotalJobs = await _context.JobPostings.CountAsync(j => j.CompanyId == company.Id);
            ViewBag.ActiveJobs = await _context.JobPostings.CountAsync(j => j.CompanyId == company.Id && j.IsActive);
            ViewBag.PendingJobs = await _context.JobPostings.CountAsync(j => j.CompanyId == company.Id && j.ApprovalStatus == "Pending");
            
            var jobIds = await _context.JobPostings
                .Where(j => j.CompanyId == company.Id)
                .Select(j => j.Id)
                .ToListAsync();
            
            ViewBag.TotalApplications = await _context.Applications
                .CountAsync(a => jobIds.Contains(a.JobPostingId));

            return View();
        }

        // Profile - GET
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            var company = await _context.CompanyProfiles
                .FirstOrDefaultAsync(c => c.UserId == user!.Id);

            return View(company);
        }

        // Profile - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(CompanyProfile model)
        {
            var user = await _userManager.GetUserAsync(User);
            var company = await _context.CompanyProfiles
                .FirstOrDefaultAsync(c => c.UserId == user!.Id);

            if (company == null)
            {
                TempData["Error"] = "Firma profili bulunamadı.";
                return RedirectToAction("Dashboard");
            }

            company.CompanyName = model.CompanyName;
            company.Industry = model.Industry;
            company.Address = model.Address;
            company.City = model.City;
            company.Phone = model.Phone;
            company.Website = model.Website;
            company.About = model.About;
            company.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Profil başarıyla güncellendi!";

            return RedirectToAction("Profile");
        }

        // MyJobs - İlanlarım
        public async Task<IActionResult> MyJobs()
        {
            var user = await _userManager.GetUserAsync(User);
            var company = await _context.CompanyProfiles
                .FirstOrDefaultAsync(c => c.UserId == user!.Id);

            if (company == null)
            {
                TempData["Error"] = "Firma profili bulunamadı.";
                return RedirectToAction("Dashboard");
            }

            var jobs = await _context.JobPostings
                .Where(j => j.CompanyId == company.Id)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            return View(jobs);
        }

        // PostJob - GET
        public IActionResult PostJob()
        {
            return View();
        }

        // PostJob - POST
        [HttpPost]
        [ActionName("PostJob")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostJobPost(JobPostingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("PostJob", model);
            }

            var user = await _userManager.GetUserAsync(User);
            var company = await _context.CompanyProfiles
                .FirstOrDefaultAsync(c => c.UserId == user!.Id);

            if (company == null)
            {
                TempData["Error"] = "Firma profili bulunamadı.";
                return RedirectToAction("Dashboard");
            }

            var job = new JobPosting
            {
                CompanyId = company.Id,
                Title = model.Title,
                Description = model.Description,
                Department = model.Department,
                City = model.City,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = model.IsActive,
                IsApproved = false, // Admin onayı bekliyor
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.JobPostings.Add(job);
            await _context.SaveChangesAsync();

            TempData["Success"] = "İlan başarıyla oluşturuldu! Admin onayından sonra yayınlanacaktır.";
            return RedirectToAction("MyJobs");
        }

        // EditJob - GET
        public async Task<IActionResult> EditJob(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var company = await _context.CompanyProfiles
                .FirstOrDefaultAsync(c => c.UserId == user!.Id);

            var job = await _context.JobPostings
                .FirstOrDefaultAsync(j => j.Id == id && j.CompanyId == company!.Id);

            if (job == null)
            {
                TempData["Error"] = "İlan bulunamadı.";
                return RedirectToAction("MyJobs");
            }

            var model = new JobPostingViewModel
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Department = job.Department,
                City = job.City,
                StartDate = job.StartDate,
                EndDate = job.EndDate,
                IsActive = job.IsActive
            };

            return View(model);
        }

        // EditJob - POST
        [HttpPost]
        [ActionName("EditJob")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditJobPost(JobPostingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditJob", model);
            }

            var user = await _userManager.GetUserAsync(User);
            var company = await _context.CompanyProfiles
                .FirstOrDefaultAsync(c => c.UserId == user!.Id);

            var job = await _context.JobPostings
                .FirstOrDefaultAsync(j => j.Id == model.Id && j.CompanyId == company!.Id);

            if (job == null)
            {
                TempData["Error"] = "İlan bulunamadı.";
                return RedirectToAction("MyJobs");
            }

            job.Title = model.Title;
            job.Description = model.Description;
            job.Department = model.Department;
            job.City = model.City;
            job.StartDate = model.StartDate;
            job.EndDate = model.EndDate;
            job.IsActive = model.IsActive;
            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["Success"] = "İlan başarıyla güncellendi!";

            return RedirectToAction("MyJobs");
        }

        // DeleteJob - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var company = await _context.CompanyProfiles
                .FirstOrDefaultAsync(c => c.UserId == user!.Id);

            var job = await _context.JobPostings
                .FirstOrDefaultAsync(j => j.Id == id && j.CompanyId == company!.Id);

            if (job == null)
            {
                TempData["Error"] = "İlan bulunamadı.";
                return RedirectToAction("MyJobs");
            }

            // Önce bu ilana ait başvuruları sil
            var applications = await _context.Applications
                .Where(a => a.JobPostingId == id)
                .ToListAsync();
            
            if (applications.Any())
            {
                _context.Applications.RemoveRange(applications);
            }

            _context.JobPostings.Remove(job);
            await _context.SaveChangesAsync();

            TempData["Success"] = "İlan ve ilgili başvurular başarıyla silindi!";
            return RedirectToAction("MyJobs");
        }

        // Applications - Başvurular
        public async Task<IActionResult> Applications(int? jobId)
        {
            var user = await _userManager.GetUserAsync(User);
            var company = await _context.CompanyProfiles
                .FirstOrDefaultAsync(c => c.UserId == user!.Id);

            if (company == null)
            {
                TempData["Error"] = "Firma profili bulunamadı.";
                return RedirectToAction("Dashboard");
            }

            var jobIds = await _context.JobPostings
                .Where(j => j.CompanyId == company.Id)
                .Select(j => j.Id)
                .ToListAsync();

            var applicationsQuery = _context.Applications
                .Include(a => a.Student)
                .ThenInclude(s => s.User)
                .Include(a => a.JobPosting)
                .Where(a => jobIds.Contains(a.JobPostingId));

            if (jobId.HasValue)
            {
                applicationsQuery = applicationsQuery.Where(a => a.JobPostingId == jobId.Value);
            }

            var applications = await applicationsQuery
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

            ViewBag.Jobs = await _context.JobPostings
                .Where(j => j.CompanyId == company.Id)
                .ToListAsync();

            return View(applications);
        }

        /// <summary>
        /// Başvuruyu kabul et
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptApplication(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var company = await _context.CompanyProfiles
                .FirstOrDefaultAsync(c => c.UserId == user!.Id);

            if (company == null)
            {
                TempData["Error"] = "Firma profili bulunamadı.";
                return RedirectToAction("Dashboard");
            }

            var application = await _context.Applications
                .Include(a => a.JobPosting)
                .Include(a => a.Student)
                .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(a => a.Id == id && a.JobPosting.CompanyId == company.Id);

            if (application == null)
            {
                TempData["Error"] = "Başvuru bulunamadı.";
                return RedirectToAction("Applications");
            }

            application.Status = "Accepted";
            application.ReviewedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{application.Student.FullName} adlı öğrencinin başvurusu kabul edildi!";
            return RedirectToAction("Applications");
        }

        /// <summary>
        /// Başvuruyu reddet
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectApplication(int id, string? note)
        {
            var user = await _userManager.GetUserAsync(User);
            var company = await _context.CompanyProfiles
                .FirstOrDefaultAsync(c => c.UserId == user!.Id);

            if (company == null)
            {
                TempData["Error"] = "Firma profili bulunamadı.";
                return RedirectToAction("Dashboard");
            }

            var application = await _context.Applications
                .Include(a => a.JobPosting)
                .Include(a => a.Student)
                .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(a => a.Id == id && a.JobPosting.CompanyId == company.Id);

            if (application == null)
            {
                TempData["Error"] = "Başvuru bulunamadı.";
                return RedirectToAction("Applications");
            }

            application.Status = "Rejected";
            application.ReviewedAt = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(note))
            {
                application.ReviewNote = note;
            }
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{application.Student.FullName} adlı öğrencinin başvurusu reddedildi.";
            return RedirectToAction("Applications");
        }

        public IActionResult Messages()
        {
            return View();
        }
    }
}

