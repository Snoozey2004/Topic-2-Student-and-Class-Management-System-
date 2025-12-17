using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EnrollmentController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // GET: Admin/Enrollment
        public IActionResult Index(string? status)
        {
            EnrollmentStatus? enrollmentStatus = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<EnrollmentStatus>(status, out var parsedStatus))
            {
                enrollmentStatus = parsedStatus;
            }

            var enrollments = _enrollmentService.GetAll(enrollmentStatus);
            ViewBag.CurrentStatus = status;
            return View(enrollments);
        }

        // POST: Admin/Enrollment/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);
            var success = _enrollmentService.Approve(id, userId);

            if (success)
            {
                TempData["SuccessMessage"] = "Phê duy?t ??ng ký thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không th? phê duy?t ??ng ký!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Enrollment/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id, string reason)
        {
            var success = _enrollmentService.Reject(id, reason);

            if (success)
            {
                TempData["SuccessMessage"] = "T? ch?i ??ng ký thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không th? t? ch?i ??ng ký!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
