using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajPortal.Data;
using StajPortal.Models;

namespace StajPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // İstatistikler
            ViewBag.TotalCompanies = await _context.CompanyProfiles.CountAsync();
            ViewBag.TotalJobs = await _context.JobPostings.Where(j => j.IsActive && j.IsApproved).CountAsync();
            ViewBag.TotalApplications = await _context.Applications.CountAsync();

            // Son 6 ilan
            var recentJobs = await _context.JobPostings
                .Include(j => j.Company)
                .Where(j => j.IsActive && j.IsApproved)
                .OrderByDescending(j => j.CreatedAt)
                .Take(6)
                .ToListAsync();

            return View(recentJobs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
