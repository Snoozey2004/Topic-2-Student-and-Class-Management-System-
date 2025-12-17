using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Areas.Lecturer.Controllers
{
    [Area("Lecturer")]
    public class DashboardController : Controller
    {
        private readonly ILecturerService _lecturerService;
        private readonly ICourseClassService _courseClassService;
        private readonly IScheduleService _scheduleService;

        public DashboardController(
            ILecturerService lecturerService,
            ICourseClassService courseClassService,
            IScheduleService scheduleService)
        {
            _lecturerService = lecturerService;
            _courseClassService = courseClassService;
            _scheduleService = scheduleService;
        }

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

            ViewBag.LecturerInfo = lecturer;
            ViewBag.TeachingClasses = _courseClassService.GetByLecturerId(lecturer.Id).Count;

            return View();
        }
    }
}
