using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EnrollmentController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // GET: Admin/Enrollment
        public IActionResult Index(string? status)
        {
            EnrollmentStatus? enrollmentStatus = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<EnrollmentStatus>(status, out var parsedStatus))
            {
                enrollmentStatus = parsedStatus;
            }

            var enrollments = _enrollmentService.GetAll(enrollmentStatus);
            ViewBag.CurrentStatus = status;
            return View(enrollments);
        }

        // GET: Admin/Enrollment/Details/5
        public IActionResult Details(int id)
        {
            var enrollments = _enrollmentService.GetAll();
            var enrollment = enrollments.FirstOrDefault(e => e.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Admin/Enrollment/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View(new EnrollmentFormViewModel());
        }

        // POST: Admin/Enrollment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EnrollmentFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = _enrollmentService.Create(model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Enrollment created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                TempData["ErrorMessage"] = "Student is already enrolled in this class!";
            }

            PopulateDropdowns();
            return View(model);
        }

        // GET: Admin/Enrollment/Edit/5
        public IActionResult Edit(int id)
        {
            var enrollment = _enrollmentService.GetById(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            var model = new EnrollmentFormViewModel
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                CourseClassId = enrollment.CourseClassId
            };

            PopulateDropdowns();
            return View(model);
        }

        // POST: Admin/Enrollment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, EnrollmentFormViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var success = _enrollmentService.Update(model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Enrollment updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                TempData["ErrorMessage"] = "Failed to update enrollment!";
            }

            PopulateDropdowns();
            return View(model);
        }

        // GET: Admin/Enrollment/Delete/5
        public IActionResult Delete(int id)
        {
            var enrollments = _enrollmentService.GetAll();
            var enrollment = enrollments.FirstOrDefault(e => e.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Admin/Enrollment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var success = _enrollmentService.Delete(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Enrollment deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete enrollment!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Enrollment/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);
            var success = _enrollmentService.Approve(id, userId);

            if (success)
            {
                TempData["SuccessMessage"] = "Enrollment approved successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve enrollment!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Enrollment/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id, string reason)
        {
            var success = _enrollmentService.Reject(id, reason);

            if (success)
            {
                TempData["SuccessMessage"] = "Enrollment rejected successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reject enrollment!";
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns()
        {
            var students = FakeDatabase.Students.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = $"{s.StudentCode} - {s.FullName}"
            }).ToList();

            var courseClasses = FakeDatabase.CourseClasses.Select(c =>
            {
                var subject = FakeDatabase.Subjects.FirstOrDefault(s => s.Id == c.SubjectId);
                return new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.ClassCode} - {subject?.SubjectName ?? ""} ({c.Semester})"
                };
            }).ToList();

            ViewBag.Students = students;
            ViewBag.CourseClasses = courseClasses;
        }
    }
}
