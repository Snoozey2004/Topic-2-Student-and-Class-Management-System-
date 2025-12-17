using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Areas.Student.Controllers
{
    [Area("Student")]
    public class GradeController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IGradeService _gradeService;

        public GradeController(IStudentService studentService, IGradeService gradeService)
        {
            _studentService = studentService;
            _gradeService = gradeService;
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

            var grades = _gradeService.GetStudentGrades(student.Id, semester);
            return View(grades);
        }
    }
}
