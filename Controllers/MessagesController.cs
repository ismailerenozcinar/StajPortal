using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajPortal.Data;
using StajPortal.Models.Entities;

namespace StajPortal.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Gelen Kutusu
        public async Task<IActionResult> Inbox()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Where(m => m.ReceiverId == user.Id)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            ViewBag.UnreadCount = messages.Count(m => !m.IsRead);
            return View(messages);
        }

        // Gönderilen Mesajlar
        public async Task<IActionResult> Sent()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var messages = await _context.Messages
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == user.Id)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            return View(messages);
        }

        // Mesaj Detayı
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var message = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.Id == id && (m.SenderId == user.Id || m.ReceiverId == user.Id));

            if (message == null)
            {
                TempData["Error"] = "Mesaj bulunamadı.";
                return RedirectToAction(nameof(Inbox));
            }

            // Okunmamışsa okundu olarak işaretle
            if (message.ReceiverId == user.Id && !message.IsRead)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return View(message);
        }

        // Yeni Mesaj - GET
        public async Task<IActionResult> Compose(string? to, string? subject, int? replyTo)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Kullanıcı listesi (Admin hariç, kendisi hariç)
            var users = await _context.Users
                .Where(u => u.Id != user.Id && u.Role != "Admin" && u.IsActive)
                .OrderBy(u => u.Role)
                .ThenBy(u => u.FullName)
                .ToListAsync();

            ViewBag.Users = users;
            ViewBag.To = to;
            ViewBag.Subject = subject;

            // Yanıt ise orijinal mesajı getir
            if (replyTo.HasValue)
            {
                var originalMessage = await _context.Messages
                    .Include(m => m.Sender)
                    .FirstOrDefaultAsync(m => m.Id == replyTo.Value);
                
                if (originalMessage != null)
                {
                    ViewBag.To = originalMessage.SenderId;
                    ViewBag.Subject = $"RE: {originalMessage.Subject}";
                    ViewBag.OriginalMessage = originalMessage;
                }
            }

            return View();
        }

        // Yeni Mesaj - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Compose(string receiverId, string? subject, string content)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(receiverId))
            {
                TempData["Error"] = "Lütfen bir alıcı seçin.";
                return RedirectToAction(nameof(Compose));
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Mesaj içeriği boş olamaz.";
                return RedirectToAction(nameof(Compose));
            }

            var receiver = await _context.Users.FindAsync(receiverId);
            if (receiver == null)
            {
                TempData["Error"] = "Alıcı bulunamadı.";
                return RedirectToAction(nameof(Compose));
            }

            var message = new Message
            {
                SenderId = user.Id,
                ReceiverId = receiverId,
                Subject = subject,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Mesajınız başarıyla gönderildi!";
            return RedirectToAction(nameof(Sent));
        }

        // Mesajı Sil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == id && (m.SenderId == user.Id || m.ReceiverId == user.Id));

            if (message == null)
            {
                TempData["Error"] = "Mesaj bulunamadı.";
                return RedirectToAction(nameof(Inbox));
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Mesaj silindi.";
            return RedirectToAction(nameof(Inbox));
        }

        // Okundu olarak işaretle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == id && m.ReceiverId == user.Id);

            if (message != null)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Inbox));
        }

        // Tümünü Okundu İşaretle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var unreadMessages = await _context.Messages
                .Where(m => m.ReceiverId == user.Id && !m.IsRead)
                .ToListAsync();

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"{unreadMessages.Count} mesaj okundu olarak işaretlendi.";
            return RedirectToAction(nameof(Inbox));
        }
    }
}
