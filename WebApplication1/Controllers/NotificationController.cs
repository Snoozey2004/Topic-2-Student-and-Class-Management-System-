using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _db;

        public NotificationController(ApplicationDbContext db)
        {
            _db = db;
        }

        // POST: /Notification/ClearAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearAll()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);

            var toRemove = _db.Notifications
                .Where(n => n.UserId == userId)
                .ToList();

            if (toRemove.Count > 0)
            {
                _db.Notifications.RemoveRange(toRemove);
                _db.SaveChanges();
            }

            return Redirect(Request.Headers.Referer.ToString());
        }
    }
}
