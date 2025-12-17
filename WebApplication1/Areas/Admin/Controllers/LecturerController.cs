using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LecturerController : Controller
    {
        private readonly ILecturerService _lecturerService;

        public LecturerController(ILecturerService lecturerService)
        {
            _lecturerService = lecturerService;
        }

        public IActionResult Index()
        {
            var lecturers = _lecturerService.GetAll();
            return View(lecturers);
        }

        // GET: Admin/Lecturer/Details/5
        public IActionResult Details(int id)
        {
            var lecturer = _lecturerService.GetDetailById(id);
            if (lecturer == null) return NotFound();
            return View(lecturer);
        }

        // GET: Admin/Lecturer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Lecturer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(WebApplication1.ViewModels.LecturerFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = _lecturerService.Create(model);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to create lecturer (duplicate email or code)");
                return View(model);
            }

            TempData["SuccessMessage"] = "Lecturer created";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Lecturer/Edit/5
        public IActionResult Edit(int id)
        {
            var l = _lecturerService.GetById(id);
            if (l == null) return NotFound();

            var model = new WebApplication1.ViewModels.LecturerFormViewModel
            {
                Id = l.Id,
                LecturerCode = l.LecturerCode,
                FullName = l.FullName,
                Email = l.Email,
                DateOfBirth = l.DateOfBirth,
                PhoneNumber = l.PhoneNumber,
                Department = l.Department,
                Title = l.Title,
                Specialization = l.Specialization,
                JoinDate = l.JoinDate
            };

            return View(model);
        }

        // POST: Admin/Lecturer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(WebApplication1.ViewModels.LecturerFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = _lecturerService.Update(model);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to update lecturer");
                return View(model);
            }

            TempData["SuccessMessage"] = "Lecturer updated";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Lecturer/Delete/5
        public IActionResult Delete(int id)
        {
            var l = _lecturerService.GetById(id);
            if (l == null) return NotFound();
            return View(l);
        }

        // POST: Admin/Lecturer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var success = _lecturerService.Delete(id);
            if (!success) TempData["ErrorMessage"] = "Unable to delete lecturer";
            else TempData["SuccessMessage"] = "Lecturer deleted";
            return RedirectToAction(nameof(Index));
        }
    }
}
