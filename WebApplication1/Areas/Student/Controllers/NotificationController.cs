using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Areas.Student.Controllers
{
    [Area("Student")]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly ApplicationDbContext _db;

        public NotificationController(INotificationService notificationService, ApplicationDbContext db)
        {
            _notificationService = notificationService;
            _db = db;
        }

        // GET: Student/Notification/Attendance
        public IActionResult Attendance()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var userId = int.Parse(userIdClaim.Value);

            var all = _notificationService.GetByUserId(userId);
            var attendance = all
                .Where(n => string.Equals(n.Type, NotificationType.Attendance.ToString(), StringComparison.OrdinalIgnoreCase))
                .ToList();

            ViewData["Title"] = "Attendance Notifications";
            ViewData["PageTitle"] = "Attendance Notifications";

            return View(attendance);
        }

        // POST: Student/Notification/ClearAttendance
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearAttendance()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var userId = int.Parse(userIdClaim.Value);

            var toRemove = _db.Notifications
                .Where(n => n.UserId == userId && n.Type == NotificationType.Attendance)
                .ToList();

            if (toRemove.Count > 0)
            {
                _db.Notifications.RemoveRange(toRemove);
                _db.SaveChanges();
            }

            TempData["SuccessMessage"] = "Attendance notifications cleared.";
            return RedirectToAction(nameof(Attendance));
        }
    }
}
