using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET: Admin/Student
        public IActionResult Index()
        {
            var students = _studentService.GetAll();
            return View(students);
        }

        // GET: Admin/Student/Details/5
        public IActionResult Details(int id)
        {
            var student = _studentService.GetDetailById(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // GET: Admin/Student/Create
        public IActionResult Create()
        {
            ViewBag.AdministrativeClasses = _studentService.GetAdministrativeClasses();
            return View();
        }

        // POST: Admin/Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StudentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AdministrativeClasses = _studentService.GetAdministrativeClasses();
                return View(model);
            }

            var success = _studentService.Create(model);
            if (!success)
            {
                ModelState.AddModelError("", "Email ho?c mã sinh viên ?ã t?n t?i");
                ViewBag.AdministrativeClasses = _studentService.GetAdministrativeClasses();
                return View(model);
            }

            TempData["SuccessMessage"] = "Thêm sinh viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Student/Edit/5
        public IActionResult Edit(int id)
        {
            var student = _studentService.GetById(id);
            if (student == null)
            {
                return NotFound();
            }

            var model = new StudentFormViewModel
            {
                Id = student.Id,
                StudentCode = student.StudentCode,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth,
                PhoneNumber = student.PhoneNumber,
                Address = student.Address,
                AdministrativeClassId = student.AdministrativeClassId,
                Major = student.Major,
                AdmissionYear = student.AdmissionYear
            };

            ViewBag.AdministrativeClasses = _studentService.GetAdministrativeClasses();
            return View(model);
        }

        // POST: Admin/Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StudentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AdministrativeClasses = _studentService.GetAdministrativeClasses();
                return View(model);
            }

            var success = _studentService.Update(model);
            if (!success)
            {
                ModelState.AddModelError("", "Email ho?c mã sinh viên ?ã t?n t?i");
                ViewBag.AdministrativeClasses = _studentService.GetAdministrativeClasses();
                return View(model);
            }

            TempData["SuccessMessage"] = "C?p nh?t sinh viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Student/Delete/5
        public IActionResult Delete(int id)
        {
            var student = _studentService.GetDetailById(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Admin/Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var success = _studentService.Delete(id);
            if (!success)
            {
                TempData["ErrorMessage"] = "Không th? xóa sinh viên!";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Xóa sinh viên thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
