using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CourseClassController : Controller
    {
        private readonly ICourseClassService _courseClassService;
        private readonly ISubjectService _subjectService;
        private readonly ILecturerService _lecturerService;

        public CourseClassController(ICourseClassService courseClassService, ISubjectService subjectService, ILecturerService lecturerService)
        {
            _courseClassService = courseClassService;
            _subjectService = subjectService;
            _lecturerService = lecturerService;
        }

        public IActionResult Index()
        {
            var classes = _courseClassService.GetAll();
            return View(classes);
        }

        // GET: Admin/CourseClass/Details/5
        public IActionResult Details(int id)
        {
            var model = _courseClassService.GetDetailById(id);
            if (model == null) return NotFound();
            return View(model);
        }

        // GET: Admin/CourseClass/Create
        public IActionResult Create()
        {
            ViewBag.Subjects = _subjectService.GetAll();
            ViewBag.Lecturers = _lecturerService.GetAll();
            return View();
        }

        // POST: Admin/CourseClass/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(WebApplication1.ViewModels.CourseClassFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Subjects = _subjectService.GetAll();
                ViewBag.Lecturers = _lecturerService.GetAll();
                return View(model);
            }

            var success = _courseClassService.Create(model);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to create course class (duplicate code)");
                ViewBag.Subjects = _subjectService.GetAll();
                ViewBag.Lecturers = _lecturerService.GetAll();
                return View(model);
            }

            TempData["SuccessMessage"] = "Course class created";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/CourseClass/Edit/5
        public IActionResult Edit(int id)
        {
            var c = _courseClassService.GetById(id);
            if (c == null) return NotFound();

            var model = new WebApplication1.ViewModels.CourseClassFormViewModel
            {
                Id = c.Id,
                ClassCode = c.ClassCode,
                SubjectId = c.SubjectId,
                LecturerId = c.LecturerId,
                Semester = c.Semester,
                MaxStudents = c.MaxStudents,
                Room = c.Room,
                Status = c.Status
            };

            ViewBag.Subjects = _subjectService.GetAll();
            ViewBag.Lecturers = _lecturerService.GetAll();
            return View(model);
        }

        // POST: Admin/CourseClass/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(WebApplication1.ViewModels.CourseClassFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Subjects = _subjectService.GetAll();
                ViewBag.Lecturers = _lecturerService.GetAll();
                return View(model);
            }

            var success = _courseClassService.Update(model);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to update course class");
                ViewBag.Subjects = _subjectService.GetAll();
                ViewBag.Lecturers = _lecturerService.GetAll();
                return View(model);
            }

            TempData["SuccessMessage"] = "Course class updated";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/CourseClass/Delete/5
        public IActionResult Delete(int id)
        {
            var c = _courseClassService.GetById(id);
            if (c == null) return NotFound();
            return View(c);
        }

        // POST: Admin/CourseClass/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var success = _courseClassService.Delete(id);
            if (!success) TempData["ErrorMessage"] = "Unable to delete course class (has enrollments)";
            else TempData["SuccessMessage"] = "Course class deleted";
            return RedirectToAction(nameof(Index));
        }
    }
}
