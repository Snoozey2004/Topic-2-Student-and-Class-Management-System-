using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Areas.Lecturer.Controllers
{
    [Area("Lecturer")]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly ILecturerService _lecturerService;

        public AttendanceController(
            IAttendanceService attendanceService,
            ILecturerService lecturerService)
        {
            _attendanceService = attendanceService;
            _lecturerService = lecturerService;
        }

        // GET: Lecturer/Attendance
        public IActionResult Index()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var userId = int.Parse(userIdClaim.Value);
            var lecturer = _lecturerService.GetByUserId(userId);

            if (lecturer == null)
            {
                return NotFound("Lecturer not found");
            }

            var classes = _attendanceService.GetClassesByLecturerId(lecturer.Id);
            return View(classes);
        }

        // GET: Lecturer/Attendance/TakeAttendance/5
        public IActionResult TakeAttendance(int id, DateTime? date, string? session)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var userId = int.Parse(userIdClaim.Value);
            var lecturer = _lecturerService.GetByUserId(userId);

            if (lecturer == null)
            {
                return NotFound("Lecturer not found");
            }

            // Default values
            var sessionDate = date ?? DateTime.Today;
            var sessionTime = session ?? "Morning";

            var model = _attendanceService.GetAttendanceSession(id, sessionDate, sessionTime);
            
            if (model.CourseClassId == 0)
            {
                return NotFound("Class not found");
            }

            return View(model);
        }

        // POST: Lecturer/Attendance/TakeAttendance
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TakeAttendance(TakeAttendanceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data. Please check and try again.";
                return RedirectToAction(nameof(TakeAttendance), new { id = model.CourseClassId });
            }

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var userId = int.Parse(userIdClaim.Value);
            var lecturer = _lecturerService.GetByUserId(userId);

            if (lecturer == null)
            {
                return NotFound("Lecturer not found");
            }

            var success = _attendanceService.TakeAttendance(model, lecturer.Id);

            if (success)
            {
                TempData["SuccessMessage"] = "Attendance recorded successfully!";
                return RedirectToAction(nameof(History), new { id = model.CourseClassId });
            }

            TempData["ErrorMessage"] = "Failed to record attendance.";
            return RedirectToAction(nameof(TakeAttendance), new { id = model.CourseClassId });
        }

        // GET: Lecturer/Attendance/History/5
        public IActionResult History(int id)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var userId = int.Parse(userIdClaim.Value);
            var lecturer = _lecturerService.GetByUserId(userId);

            if (lecturer == null)
            {
                return NotFound("Lecturer not found");
            }

            var history = _attendanceService.GetAttendanceHistory(id);

            if (history.CourseClassId == 0)
            {
                return NotFound("Class not found");
            }

            return View(history);
        }
    }
}
