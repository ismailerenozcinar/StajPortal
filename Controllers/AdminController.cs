using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajPortal.Data;

namespace StajPortal.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            // İstatistikler
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalStudents = await _context.Users.CountAsync(u => u.Role == "Student");
            ViewBag.TotalCompanies = await _context.Users.CountAsync(u => u.Role == "Company");
            ViewBag.TotalJobs = await _context.JobPostings.CountAsync();
            ViewBag.PendingJobs = await _context.JobPostings.CountAsync(j => !j.IsApproved);
            ViewBag.TotalApplications = await _context.Applications.CountAsync();

            return View();
        }

        public IActionResult Users()
        {
            return View();
        }

        public IActionResult Jobs()
        {
            return View();
        }

        public async Task<IActionResult> JobApprovals()
        {
            var pendingJobs = await _context.JobPostings
                .Include(j => j.Company)
                    .ThenInclude(c => c.User)
                .Where(j => !j.IsApproved)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            return View(pendingJobs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveJob(int id)
        {
            var job = await _context.JobPostings.FindAsync(id);
            if (job == null)
            {
                TempData["Error"] = "İlan bulunamadı.";
                return RedirectToAction(nameof(JobApprovals));
            }

            job.IsApproved = true;
            job.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"'{job.Title}' ilanı onaylandı!";
            return RedirectToAction(nameof(JobApprovals));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectJob(int id)
        {
            var job = await _context.JobPostings.FindAsync(id);
            if (job == null)
            {
                TempData["Error"] = "İlan bulunamadı.";
                return RedirectToAction(nameof(JobApprovals));
            }

            job.IsApproved = false;
            job.IsActive = false; // Reddedilen ilanı deaktif et
            job.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"'{job.Title}' ilanı reddedildi.";
            return RedirectToAction(nameof(JobApprovals));
        }

        public IActionResult Reports()
        {
            return View();
        }
    }
}

