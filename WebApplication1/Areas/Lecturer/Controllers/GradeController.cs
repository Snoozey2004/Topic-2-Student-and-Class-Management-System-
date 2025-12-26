using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Areas.Lecturer.Controllers
{
    [Area("Lecturer")]
    public class GradeController : Controller
    {
        private readonly ILecturerService _lecturerService;
        private readonly ICourseClassService _courseClassService;
        private readonly IGradeService _gradeService;
        private readonly ISubjectService _subjectService;

        public GradeController(
            ILecturerService lecturerService,
            ICourseClassService courseClassService,
            IGradeService gradeService,
            ISubjectService subjectService)
        {
            _lecturerService = lecturerService;
            _courseClassService = courseClassService;
            _gradeService = gradeService;
            _subjectService = subjectService;
        }

        // GET: Lecturer/Grade/Index
        public IActionResult Index()
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

            var classes = _courseClassService.GetByLecturerId(lecturer.Id);
            return View(classes);
        }

        // GET: Lecturer/Grade/CourseClass/5
        public IActionResult CourseClass(int id)
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

            // Ki?m tra quy?n truy c?p
            var courseClass = _courseClassService.GetById(id);
            if (courseClass == null || courseClass.LecturerId != lecturer.Id)
            {
                return Forbid();
            }

            var grades = _gradeService.GetGradesByCourseClass(id);
            var subject = _subjectService.GetById(courseClass.SubjectId);
            
            var viewModel = new ClassGradeListViewModel
            {
                CourseClassId = courseClass.Id,
                ClassCode = courseClass.ClassCode,
                SubjectName = subject?.SubjectName ?? "",
                Semester = courseClass.Semester,
                Grades = grades
            };

            return View(viewModel);
        }

        // POST: Lecturer/Grade/UpdateGrades
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateGrades(List<GradeInputViewModel> grades, int courseClassId)
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

            var success = _gradeService.BatchUpdateGrades(grades, lecturer.Id);

            if (success)
            {
                TempData["SuccessMessage"] = "C?p nh?t ði?m thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Có l?i x?y ra khi c?p nh?t ði?m!";
            }

            return RedirectToAction(nameof(CourseClass), new { id = courseClassId });
        }
    }
}
