using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Areas.Student.Controllers
{
    [Area("Student")]
    public class EnrollmentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IStudentService studentService, IEnrollmentService enrollmentService)
        {
            _studentService = studentService;
            _enrollmentService = enrollmentService;
        }

        // GET: Student/Enrollment
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

            // L?y danh sách môn h?c có th? ??ng ký
            var availableCourses = _enrollmentService.GetAvailableCoursesForStudent(student.Id);

            // L?y danh sách ??ng ký c?a sinh viên
            var myEnrollments = _enrollmentService.GetByStudentId(student.Id);

            var viewModel = new ViewModels.EnrollmentRegisterViewModel
            {
                AvailableCourses = availableCourses,
                MyEnrollments = myEnrollments
            };

            return View(viewModel);
        }

        // POST: Student/Enrollment/Enroll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Enroll(int courseClassId)
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

            var success = _enrollmentService.Enroll(student.Id, courseClassId);

            if (success)
            {
                TempData["SuccessMessage"] = "??ng ký môn h?c thành công! ??n ??ng ký ?ang ch? ???c duy?t.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không th? ??ng ký môn h?c. Vui lòng ki?m tra l?i ?i?u ki?n.";
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
                TempData["SuccessMessage"] = "H?y ??ng ký thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không th? h?y ??ng ký!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
