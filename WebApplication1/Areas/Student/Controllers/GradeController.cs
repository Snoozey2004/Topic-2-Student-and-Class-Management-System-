using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Services;

namespace WebApplication1.Areas.Student.Controllers
{
    [Area("Student")]
    public class GradeController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IGradeService _gradeService;
        private readonly ISemesterService _semesterService;

        public GradeController(
            IStudentService studentService, 
            IGradeService gradeService,
            ISemesterService semesterService)
        {
            _studentService = studentService;
            _gradeService = gradeService;
            _semesterService = semesterService;
        }

        // GET: Student/Grade
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

            // Get semester data from SemesterService
            var currentSemester = semester ?? _semesterService.GetCurrentSemester();
            var allSemesters = _semesterService.GetAllSemesters();

            ViewBag.Semesters = new SelectList(allSemesters);
            ViewBag.CurrentSemester = currentSemester;

            var grades = _gradeService.GetStudentGrades(student.Id, currentSemester);
            return View(grades);
        }
    }
}
