using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Areas.Student.Controllers
{
    [Area("Student")]
    public class ScheduleController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IStudentService studentService, IScheduleService scheduleService)
        {
            _studentService = studentService;
            _scheduleService = scheduleService;
        }

        // GET: Student/Schedule
        public IActionResult Index(string? semester)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);
            var student = _studentService.GetByUserId(userId);

            if (student == null)
            {
                return NotFound();
            }

            var semesters = _scheduleService.GetStudentSemesters(student.Id);
            var timetable = _scheduleService.GetStudentTimetable(student.Id, semester);

            ViewBag.Semesters = semesters;
            ViewBag.SelectedSemester = timetable.Semester;

            return View(timetable);
        }
    }
}
