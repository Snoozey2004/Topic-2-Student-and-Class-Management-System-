using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;
using WebApplication1.Services;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GradeController : Controller
    {
        private readonly IGradeService _gradeService;

        public GradeController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        // GET: Admin/Grade
        public IActionResult Index()
        {
            var grades = _gradeService.GetAll();
            return View(grades);
        }

        // GET: Admin/Grade/Details/5
        public IActionResult Details(int id)
        {
            var grade = FakeDatabase.Grades.FirstOrDefault(g => g.Id == id);
            if (grade == null)
            {
                return NotFound();
            }

            var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == grade.StudentId);
            var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == grade.CourseClassId);
            var subject = courseClass != null 
                ? FakeDatabase.Subjects.FirstOrDefault(s => s.Id == courseClass.SubjectId) 
                : null;

            var model = new GradeDetailAdminViewModel
            {
                Id = grade.Id,
                StudentCode = student?.StudentCode ?? "",
                StudentName = student?.FullName ?? "",
                ClassCode = courseClass?.ClassCode ?? "",
                SubjectName = subject?.SubjectName ?? "",
                AttendanceScore = grade.AttendanceScore,
                MidtermScore = grade.MidtermScore,
                FinalScore = grade.FinalScore,
                TotalScore = grade.TotalScore,
                LetterGrade = grade.LetterGrade,
                IsPassed = grade.IsPassed,
                LastUpdated = grade.LastUpdated
            };

            return View(model);
        }

        // GET: Admin/Grade/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View(new GradeFormViewModel());
        }

        // POST: Admin/Grade/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GradeFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if grade already exists for this enrollment
                var existingGrade = FakeDatabase.Grades.FirstOrDefault(g => g.EnrollmentId == model.EnrollmentId);
                if (existingGrade != null)
                {
                    TempData["ErrorMessage"] = "Grade already exists for this enrollment!";
                    PopulateDropdowns();
                    return View(model);
                }

                // Get enrollment info
                var enrollment = FakeDatabase.Enrollments.FirstOrDefault(e => e.Id == model.EnrollmentId);
                if (enrollment == null)
                {
                    TempData["ErrorMessage"] = "Enrollment not found!";
                    PopulateDropdowns();
                    return View(model);
                }

                var grade = new Grade
                {
                    Id = FakeDatabase.GetNextGradeId(),
                    EnrollmentId = model.EnrollmentId,
                    StudentId = enrollment.StudentId,
                    CourseClassId = enrollment.CourseClassId,
                    AttendanceScore = model.AttendanceScore,
                    MidtermScore = model.MidtermScore,
                    FinalScore = model.FinalScore,
                    LastUpdated = DateTime.Now,
                    UpdatedBy = 1
                };

                // Calculate total score if all components present
                if (grade.AttendanceScore.HasValue && grade.MidtermScore.HasValue && grade.FinalScore.HasValue)
                {
                    grade.TotalScore = Math.Round(
                        grade.AttendanceScore.Value * 0.1 +
                        grade.MidtermScore.Value * 0.3 +
                        grade.FinalScore.Value * 0.6, 2);
                    grade.LetterGrade = _gradeService.CalculateLetterGrade(grade.TotalScore.Value);
                    grade.IsPassed = grade.TotalScore.Value >= 4.0;
                }

                FakeDatabase.Grades.Add(grade);
                TempData["SuccessMessage"] = "Grade created successfully!";
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns();
            return View(model);
        }

        // GET: Admin/Grade/Edit/5
        public IActionResult Edit(int id)
        {
            var grade = FakeDatabase.Grades.FirstOrDefault(g => g.Id == id);
            if (grade == null)
            {
                return NotFound();
            }

            var model = new GradeFormViewModel
            {
                Id = grade.Id,
                EnrollmentId = grade.EnrollmentId,
                StudentId = grade.StudentId,
                CourseClassId = grade.CourseClassId,
                AttendanceScore = grade.AttendanceScore,
                MidtermScore = grade.MidtermScore,
                FinalScore = grade.FinalScore
            };

            PopulateDropdowns();
            return View(model);
        }

        // POST: Admin/Grade/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, GradeFormViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var grade = FakeDatabase.Grades.FirstOrDefault(g => g.Id == id);
                if (grade == null)
                {
                    return NotFound();
                }

                grade.AttendanceScore = model.AttendanceScore;
                grade.MidtermScore = model.MidtermScore;
                grade.FinalScore = model.FinalScore;
                grade.LastUpdated = DateTime.Now;

                // Calculate total score if all components present
                if (grade.AttendanceScore.HasValue && grade.MidtermScore.HasValue && grade.FinalScore.HasValue)
                {
                    grade.TotalScore = Math.Round(
                        grade.AttendanceScore.Value * 0.1 +
                        grade.MidtermScore.Value * 0.3 +
                        grade.FinalScore.Value * 0.6, 2);
                    grade.LetterGrade = _gradeService.CalculateLetterGrade(grade.TotalScore.Value);
                    grade.IsPassed = grade.TotalScore.Value >= 4.0;
                }

                TempData["SuccessMessage"] = "Grade updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns();
            return View(model);
        }

        // GET: Admin/Grade/Delete/5
        public IActionResult Delete(int id)
        {
            var grades = _gradeService.GetAll();
            var grade = grades.FirstOrDefault(g => g.Id == id);
            if (grade == null)
            {
                return NotFound();
            }

            return View(grade);
        }

        // POST: Admin/Grade/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var grade = FakeDatabase.Grades.FirstOrDefault(g => g.Id == id);
            if (grade == null)
            {
                TempData["ErrorMessage"] = "Grade not found!";
                return RedirectToAction(nameof(Index));
            }

            FakeDatabase.Grades.Remove(grade);
            TempData["SuccessMessage"] = "Grade deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns()
        {
            // Get approved enrollments that don't have grades yet
            var enrollmentsWithoutGrades = FakeDatabase.Enrollments
                .Where(e => e.Status == EnrollmentStatus.Approved)
                .Where(e => !FakeDatabase.Grades.Any(g => g.EnrollmentId == e.Id))
                .Select(e =>
                {
                    var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == e.StudentId);
                    var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == e.CourseClassId);
                    var subject = courseClass != null 
                        ? FakeDatabase.Subjects.FirstOrDefault(s => s.Id == courseClass.SubjectId) 
                        : null;
                    return new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = $"{student?.StudentCode} - {student?.FullName} | {courseClass?.ClassCode} - {subject?.SubjectName}"
                    };
                }).ToList();

            ViewBag.Enrollments = enrollmentsWithoutGrades;
        }
    }
}
