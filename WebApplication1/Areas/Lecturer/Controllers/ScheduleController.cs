using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Areas.Lecturer.Controllers
{
    [Area("Lecturer")]
    public class ScheduleController : Controller
    {
        private readonly ILecturerService _lecturerService;
        private readonly IScheduleService _scheduleService;

        public ScheduleController(ILecturerService lecturerService, IScheduleService scheduleService)
        {
            _lecturerService = lecturerService;
            _scheduleService = scheduleService;
        }

        // GET: Lecturer/Schedule
        public IActionResult Index(string? semester)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);
            var lecturer = _lecturerService.GetByUserId(userId);

            if (lecturer == null)
            {
                return NotFound();
            }

            var semesters = _scheduleService.GetLecturerSemesters(lecturer.Id);
            var timetable = _scheduleService.GetLecturerTimetable(lecturer.Id, semester);

            ViewBag.Semesters = semesters;
            ViewBag.SelectedSemester = timetable.Semester;

            return View(timetable);
        }
    }
}
