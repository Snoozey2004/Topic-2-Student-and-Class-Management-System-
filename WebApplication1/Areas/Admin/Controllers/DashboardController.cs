using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly ILecturerService _lecturerService;
        private readonly ISubjectService _subjectService;
        private readonly ICourseClassService _courseClassService;
        private readonly IEnrollmentService _enrollmentService;

        public DashboardController(
            IUserService userService,
            IStudentService studentService,
            ILecturerService lecturerService,
            ISubjectService subjectService,
            ICourseClassService courseClassService,
            IEnrollmentService enrollmentService)
        {
            _userService = userService;
            _studentService = studentService;
            _lecturerService = lecturerService;
            _subjectService = subjectService;
            _courseClassService = courseClassService;
            _enrollmentService = enrollmentService;
        }

        public IActionResult Index()
        {
            ViewBag.TotalUsers = _userService.GetAll().Count;
            ViewBag.TotalStudents = _studentService.GetAll().Count;
            ViewBag.TotalLecturers = _lecturerService.GetAll().Count;
            ViewBag.TotalSubjects = _subjectService.GetAll().Count;
            ViewBag.TotalCourseClasses = _courseClassService.GetAll().Count;
            ViewBag.PendingEnrollments = _enrollmentService.GetAll(Models.EnrollmentStatus.Pending).Count;

            return View();
        }
    }
}
