using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Areas.Student.Controllers
{
    [Area("Student")]
    public class DashboardController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IGradeService _gradeService;
        private readonly INotificationService _notificationService;

        public DashboardController(
            IStudentService studentService,
            IEnrollmentService enrollmentService,
            IGradeService gradeService,
            INotificationService notificationService)
        {
            _studentService = studentService;
            _enrollmentService = enrollmentService;
            _gradeService = gradeService;
            _notificationService = notificationService;
        }

        public IActionResult Index()
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

            ViewBag.StudentInfo = student;
            ViewBag.EnrollmentCount = _enrollmentService.GetByStudentId(student.Id).Count;
            ViewBag.GPA = _studentService.CalculateGPA(student.Id);
            ViewBag.UnreadNotifications = _notificationService.GetUnreadCount(userId);

            return View();
        }
    }
}
