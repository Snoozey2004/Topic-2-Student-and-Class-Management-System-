using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubjectController : Controller
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        public IActionResult Index()
        {
            var subjects = _subjectService.GetAll();
            return View(subjects);
        }

        // GET: Admin/Subject/Details/5
        public IActionResult Details(int id)
        {
            var subject = _subjectService.GetDetailById(id);
            if (subject == null) return NotFound();
            return View(subject);
        }

        // GET: Admin/Subject/Create
        public IActionResult Create()
        {
            ViewBag.AllSubjects = _subjectService.GetAll();
            return View();
        }

        // POST: Admin/Subject/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(WebApplication1.ViewModels.SubjectFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AllSubjects = _subjectService.GetAll();
                return View(model);
            }

            var success = _subjectService.Create(model);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to create subject (duplicate code)");
                ViewBag.AllSubjects = _subjectService.GetAll();
                return View(model);
            }

            TempData["SuccessMessage"] = "Subject created";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Subject/Edit/5
        public IActionResult Edit(int id)
        {
            var subject = _subjectService.GetById(id);
            if (subject == null) return NotFound();

            var model = new WebApplication1.ViewModels.SubjectFormViewModel
            {
                Id = subject.Id,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Credits = subject.Credits,
                Department = subject.Department,
                Description = subject.Description,
                PrerequisiteSubjectIds = subject.PrerequisiteSubjectIds ?? new List<int>()
            };

            ViewBag.AllSubjects = _subjectService.GetAll().Where(s => s.Id != id).ToList();
            return View(model);
        }

        // POST: Admin/Subject/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(WebApplication1.ViewModels.SubjectFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AllSubjects = _subjectService.GetAll().Where(s => s.Id != model.Id).ToList();
                return View(model);
            }

            var success = _subjectService.Update(model);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to update subject");
                ViewBag.AllSubjects = _subjectService.GetAll().Where(s => s.Id != model.Id).ToList();
                return View(model);
            }

            TempData["SuccessMessage"] = "Subject updated";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Subject/Delete/5
        public IActionResult Delete(int id)
        {
            var subject = _subjectService.GetById(id);
            if (subject == null) return NotFound();
            return View(subject);
        }

        // POST: Admin/Subject/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var success = _subjectService.Delete(id);
            if (!success) TempData["ErrorMessage"] = "Unable to delete subject (in use)";
            else TempData["SuccessMessage"] = "Subject deleted";
            return RedirectToAction(nameof(Index));
        }
    }
}
