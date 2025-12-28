using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajPortal.Data;
using StajPortal.Models.Entities;

namespace StajPortal.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            // İstatistikler
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalStudents = await _context.Users.CountAsync(u => u.Role == "Student");
            ViewBag.TotalCompanies = await _context.Users.CountAsync(u => u.Role == "Company");
            ViewBag.TotalJobs = await _context.JobPostings.CountAsync();
            ViewBag.PendingJobs = await _context.JobPostings.CountAsync(j => j.ApprovalStatus == "Pending");
            ViewBag.TotalApplications = await _context.Applications.CountAsync();

            return View();
        }

        public async Task<IActionResult> Users(string? role, string? status, string? search)
        {
            var usersQuery = _context.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.CompanyProfile)
                .AsQueryable();

            // Filtreleme: Rol
            if (!string.IsNullOrWhiteSpace(role))
            {
                usersQuery = usersQuery.Where(u => u.Role == role);
                ViewBag.SelectedRole = role;
            }

            // Filtreleme: Durum
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "active")
                    usersQuery = usersQuery.Where(u => u.IsActive);
                else if (status == "inactive")
                    usersQuery = usersQuery.Where(u => !u.IsActive);
                ViewBag.SelectedStatus = status;
            }

            // Filtreleme: Arama
            if (!string.IsNullOrWhiteSpace(search))
            {
                usersQuery = usersQuery.Where(u =>
                    (u.FullName != null && u.FullName.Contains(search)) ||
                    (u.Email != null && u.Email.Contains(search)));
                ViewBag.SearchTerm = search;
            }

            var users = await usersQuery
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            // İstatistikler
            ViewBag.TotalCount = await _context.Users.CountAsync();
            ViewBag.StudentCount = await _context.Users.CountAsync(u => u.Role == "Student");
            ViewBag.CompanyCount = await _context.Users.CountAsync(u => u.Role == "Company");
            ViewBag.AdminCount = await _context.Users.CountAsync(u => u.Role == "Admin");
            ViewBag.ActiveCount = await _context.Users.CountAsync(u => u.IsActive);
            ViewBag.InactiveCount = await _context.Users.CountAsync(u => !u.IsActive);

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Users));
            }

            // Admin kendini deaktif edemez
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == id)
            {
                TempData["Error"] = "Kendi hesabınızı deaktif edemezsiniz.";
                return RedirectToAction(nameof(Users));
            }

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            var statusMessage = user.IsActive ? "aktif" : "pasif";
            TempData["Success"] = $"'{user.FullName ?? user.Email}' kullanıcısı {statusMessage} yapıldı.";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Users));
            }

            // Admin kendini silemez
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == id)
            {
                TempData["Error"] = "Kendi hesabınızı silemezsiniz.";
                return RedirectToAction(nameof(Users));
            }

            // Admin kullanıcı silinemez
            if (user.Role == "Admin")
            {
                TempData["Error"] = "Admin kullanıcılar silinemez.";
                return RedirectToAction(nameof(Users));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = $"'{user.FullName ?? user.Email}' kullanıcısı silindi.";
            }
            else
            {
                TempData["Error"] = "Kullanıcı silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Users));
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails(string id)
        {
            var user = await _context.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.CompanyProfile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                TempData["Error"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Users));
            }

            // Kullanıcıya ait istatistikler
            if (user.Role == "Student" && user.StudentProfile != null)
            {
                ViewBag.ApplicationCount = await _context.Applications
                    .CountAsync(a => a.StudentId == user.StudentProfile.Id);
                ViewBag.AcceptedCount = await _context.Applications
                    .CountAsync(a => a.StudentId == user.StudentProfile.Id && a.Status == "Accepted");
            }
            else if (user.Role == "Company" && user.CompanyProfile != null)
            {
                ViewBag.JobCount = await _context.JobPostings
                    .CountAsync(j => j.CompanyId == user.CompanyProfile.Id);
                ViewBag.ApplicationCount = await _context.Applications
                    .Include(a => a.JobPosting)
                    .CountAsync(a => a.JobPosting.CompanyId == user.CompanyProfile.Id);
            }

            return View(user);
        }

        public IActionResult Jobs()
        {
            return View();
        }

        public async Task<IActionResult> JobApprovals()
        {
            // Onay bekleyen ilanlar: IsApproved=false VE IsActive=true
            // Reddedilen ilanlar IsActive=false olduğu için listede görünmez
            var pendingJobs = await _context.JobPostings
                .Include(j => j.Company)
                    .ThenInclude(c => c.User)
                .Where(j => !j.IsApproved && j.IsActive)
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
            job.ApprovalStatus = "Approved";
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
            job.ApprovalStatus = "Rejected";
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

