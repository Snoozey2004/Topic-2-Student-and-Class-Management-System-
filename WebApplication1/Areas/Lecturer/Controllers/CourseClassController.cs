using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Areas.Lecturer.Controllers
{
    [Area("Lecturer")]
    public class CourseClassController : Controller
    {
        private readonly ILecturerService _lecturerService;
        private readonly ICourseClassService _courseClassService;

        public CourseClassController(ILecturerService lecturerService, ICourseClassService courseClassService)
        {
            _lecturerService = lecturerService;
            _courseClassService = courseClassService;
        }

        // GET: Lecturer/CourseClass
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

        // GET: Lecturer/CourseClass/Details/5
        public IActionResult Details(int id)
        {
            var courseClass = _courseClassService.GetDetailById(id);
            if (courseClass == null)
            {
                return NotFound();
            }

            // Ki?m tra xem lecturer có quy?n xem l?p này không
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null)
            {
                var userId = int.Parse(userIdClaim.Value);
                var lecturer = _lecturerService.GetByUserId(userId);
                
                var actualClass = _courseClassService.GetById(id);
                if (actualClass != null && actualClass.LecturerId != lecturer?.Id)
                {
                    return Forbid();
                }
            }

            return View(courseClass);
        }
    }
}
