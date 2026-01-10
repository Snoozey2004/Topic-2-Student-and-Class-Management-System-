using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Areas.Lecturer.Controllers
{
    [Area("Lecturer")]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly ILecturerService _lecturerService;
        private readonly ISemesterService _semesterService;
        private readonly IScheduleService _scheduleService;
        private readonly ApplicationDbContext _db;

        public AttendanceController(
            IAttendanceService attendanceService,
            ILecturerService lecturerService,
            ISemesterService semesterService,
            IScheduleService scheduleService,
            ApplicationDbContext db)
        {
            _attendanceService = attendanceService;
            _lecturerService = lecturerService;
            _semesterService = semesterService;
            _scheduleService = scheduleService;
            _db = db;
        }

        // GET: Lecturer/Attendance
        public IActionResult Index(string? semester = null)
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

            // Get semester data from SemesterService
            var currentSemester = semester ?? _semesterService.GetCurrentSemester();
            var allSemesters = _semesterService.GetAllSemesters();

            ViewBag.Semesters = new SelectList(allSemesters);
            ViewBag.CurrentSemester = currentSemester;

            var classes = _attendanceService.GetClassesByLecturerId(lecturer.Id, currentSemester);
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

            var sessionDate = (date ?? DateTime.Today).Date;
            var sessionTime = (session ?? "Morning").Trim();

            // Allow editing if attendance already exists for this session
            var alreadyExists = _db.Attendances
                .AsNoTracking()
                .Any(a => a.CourseClassId == id && a.AttendanceDate.Date == sessionDate && a.Session == sessionTime);

            if (!alreadyExists)
            {
                // For new sessions, must match scheduled day-of-week + session
                var scheduled = _scheduleService.GetByCourseClassId(id);

                var matches = scheduled.Any(s =>
                    string.Equals(s.DayOfWeek, sessionDate.DayOfWeek.ToString(), StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(s.Session, sessionTime, StringComparison.OrdinalIgnoreCase));

                if (!matches)
                {
                    var allowed = scheduled
                        .GroupBy(s => s.DayOfWeek, StringComparer.OrdinalIgnoreCase)
                        .Select(g => $"{g.Key} ({string.Join("/", g.Select(x => x.Session).Distinct(StringComparer.OrdinalIgnoreCase))})")
                        .ToList();

                    TempData["ErrorMessage"] = allowed.Count > 0
                        ? $"You can only take attendance on scheduled sessions: {string.Join(", ", allowed)}."
                        : "No schedule found for this class. Please set up a schedule first.";

                    return RedirectToAction(nameof(Index));
                }
            }

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
                return RedirectToAction(nameof(TakeAttendance), new { id = model.CourseClassId, date = model.SessionDate, session = model.Session });
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

            var presentSet = (model.PresentStudentIds ?? new List<int>()).ToHashSet();
            foreach (var s in model.Students)
            {
                s.IsPresent = presentSet.Contains(s.StudentId);
            }

            var success = _attendanceService.TakeAttendance(model, lecturer.Id);

            if (success)
            {
                TempData["SuccessMessage"] = "Attendance recorded successfully!";
                return RedirectToAction(nameof(History), new { id = model.CourseClassId });
            }

            TempData["ErrorMessage"] = "Attendance can only be recorded on a scheduled class day.";
            return RedirectToAction(nameof(TakeAttendance), new { id = model.CourseClassId, date = model.SessionDate, session = model.Session });
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
