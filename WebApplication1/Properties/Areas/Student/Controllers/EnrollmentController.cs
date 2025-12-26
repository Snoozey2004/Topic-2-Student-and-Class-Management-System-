using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Services;

namespace WebApplication1.Areas.Student.Controllers
{
    [Area("Student")]
    public class EnrollmentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly ISemesterService _semesterService;

        public EnrollmentController(
            IStudentService studentService, 
            IEnrollmentService enrollmentService,
            ISemesterService semesterService)
        {
            _studentService = studentService;
            _enrollmentService = enrollmentService;
            _semesterService = semesterService;
        }

        // GET: Student/Enrollment
        public IActionResult Index(string? semester = null)
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

            // Get semester data from SemesterService (no hardcode!)
            var currentSemester = semester ?? _semesterService.GetCurrentSemester();
            var allSemesters = _semesterService.GetAllSemesters();

            // Pass to ViewBag for dropdown
            ViewBag.Semesters = new SelectList(allSemesters);
            ViewBag.CurrentSemester = currentSemester;

            // Get available courses with semester from controller
            var availableCourses = _enrollmentService.GetAvailableCoursesForStudent(student.Id, currentSemester);

            // Get student's enrollments
            var myEnrollments = _enrollmentService.GetByStudentId(student.Id);

            var viewModel = new ViewModels.EnrollmentRegisterViewModel
            {
                AvailableCourses = availableCourses,
                MyEnrollments = myEnrollments
            };

            return View(viewModel);
        }

        // POST: Student/Enrollment/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(int courseClassId)
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

            // Auto-approve = true (like real university systems)
            var success = _enrollmentService.Enroll(student.Id, courseClassId, autoApprove: true);

            if (success)
            {
                TempData["SuccessMessage"] = "Registration successful! The course has been added to your schedule.";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to register for this course. Please check the requirements.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Student/Enrollment/Drop
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Drop(int enrollmentId)
        {
            var success = _enrollmentService.Drop(enrollmentId);

            if (success)
            {
                TempData["SuccessMessage"] = "Course dropped successfully! The course has been removed from your schedule.";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to drop this course.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
