using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajPortal.Data;
using StajPortal.Models.Entities;

namespace StajPortal.Controllers
{
    [Authorize(Policy = "StudentOnly")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var student = await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null) return RedirectToAction("Login", "Account");

            // İstatistikler
            var totalApplications = await _context.Applications
                .CountAsync(a => a.StudentId == student.Id);

            var pendingApplications = await _context.Applications
                .CountAsync(a => a.StudentId == student.Id && a.Status == "Pending");

            var acceptedApplications = await _context.Applications
                .CountAsync(a => a.StudentId == student.Id && a.Status == "Accepted");

            var activeJobs = await _context.JobPostings
                .CountAsync(j => j.IsActive && j.IsApproved);

            ViewBag.TotalApplications = totalApplications;
            ViewBag.PendingApplications = pendingApplications;
            ViewBag.AcceptedApplications = acceptedApplications;
            ViewBag.ActiveJobs = activeJobs;

            // Son başvurular
            var recentApplications = await _context.Applications
                .Include(a => a.JobPosting)
                    .ThenInclude(j => j.Company)
                .Where(a => a.StudentId == student.Id)
                .OrderByDescending(a => a.AppliedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentApplications = recentApplications;

            return View(student);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var student = await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(StudentProfile model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var student = await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null) return RedirectToAction("Login", "Account");

            // Güncelle
            student.FullName = model.FullName;
            student.University = model.University;
            student.Department = model.Department;
            student.GraduationYear = model.GraduationYear;
            student.GPA = model.GPA;
            student.Phone = model.Phone;
            student.City = model.City;
            student.About = model.About;
            student.CVLink = model.CVLink;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi!";
            return RedirectToAction(nameof(Profile));
        }

        public async Task<IActionResult> Applications()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var student = await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null) return RedirectToAction("Login", "Account");

            var applications = await _context.Applications
                .Include(a => a.JobPosting)
                    .ThenInclude(j => j.Company)
                .Where(a => a.StudentId == student.Id)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

            return View(applications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int jobId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Giriş yapmanız gerekiyor.";
                return RedirectToAction("Login", "Account");
            }

            var student = await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null)
            {
                TempData["Error"] = "Öğrenci profili bulunamadı.";
                return RedirectToAction("Profile");
            }

            // İlan var mı ve aktif mi kontrol et
            var job = await _context.JobPostings
                .FirstOrDefaultAsync(j => j.Id == jobId && j.IsActive && j.IsApproved);

            if (job == null)
            {
                TempData["Error"] = "İlan bulunamadı veya artık aktif değil.";
                return RedirectToAction("Details", "Jobs", new { id = jobId });
            }

            // Daha önce başvurmuş mu kontrol et
            var existingApplication = await _context.Applications
                .AnyAsync(a => a.JobPostingId == jobId && a.StudentId == student.Id);

            if (existingApplication)
            {
                TempData["Warning"] = "Bu ilana zaten başvurdunuz.";
                return RedirectToAction("Details", "Jobs", new { id = jobId });
            }

            // Yeni başvuru oluştur
            var application = new Models.Entities.Application
            {
                JobPostingId = jobId,
                StudentId = student.Id,
                Status = "Pending",
                AppliedAt = DateTime.UtcNow
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Başvurunuz başarıyla gönderildi!";
            return RedirectToAction("Applications");
        }

        public IActionResult Messages()
        {
            return View();
        }
    }
}

