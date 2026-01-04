using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GradeController : Controller
    {
        private readonly IGradeService _gradeService;
        private readonly ApplicationDbContext _db;

        public GradeController(IGradeService gradeService, ApplicationDbContext db)
        {
            _gradeService = gradeService;
            _db = db;
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
            var grade = _db.Grades.AsNoTracking().FirstOrDefault(g => g.Id == id);
            if (grade == null) return NotFound();

            var student = _db.Students.AsNoTracking().FirstOrDefault(s => s.Id == grade.StudentId);
            var courseClass = _db.CourseClasses.AsNoTracking().FirstOrDefault(c => c.Id == grade.CourseClassId);
            var subject = courseClass != null
                ? _db.Subjects.AsNoTracking().FirstOrDefault(s => s.Id == courseClass.SubjectId)
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
                var existingGrade = _db.Grades.FirstOrDefault(g => g.EnrollmentId == model.EnrollmentId);
                if (existingGrade != null)
                {
                    TempData["ErrorMessage"] = "Grade already exists for this enrollment!";
                    PopulateDropdowns();
                    return View(model);
                }

                // Get enrollment info
                var enrollment = _db.Enrollments.AsNoTracking().FirstOrDefault(e => e.Id == model.EnrollmentId);
                if (enrollment == null)
                {
                    TempData["ErrorMessage"] = "Enrollment not found!";
                    PopulateDropdowns();
                    return View(model);
                }

                var grade = new Grade
                {
                    // KHÔNG set Id nếu Identity
                    EnrollmentId = model.EnrollmentId,
                    StudentId = enrollment.StudentId,
                    CourseClassId = enrollment.CourseClassId,
                    AttendanceScore = model.AttendanceScore,
                    MidtermScore = model.MidtermScore,
                    FinalScore = model.FinalScore,
                    LastUpdated = DateTime.Now,
                    UpdatedBy = 1 // TODO: lấy userId admin thật nếu có claim
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

                _db.Grades.Add(grade);
                _db.SaveChanges();

                TempData["SuccessMessage"] = "Grade created successfully!";
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns();
            return View(model);
        }

        // GET: Admin/Grade/Edit/5
        public IActionResult Edit(int id)
        {
            var grade = _db.Grades.AsNoTracking().FirstOrDefault(g => g.Id == id);
            if (grade == null) return NotFound();

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
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var grade = _db.Grades.FirstOrDefault(g => g.Id == id);
                if (grade == null) return NotFound();

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
                else
                {
                    // Nếu thiếu điểm thì clear để khỏi giữ dữ liệu cũ
                    grade.TotalScore = null;
                    grade.LetterGrade = null;
                    grade.IsPassed = false;
                }

                _db.SaveChanges();

                TempData["SuccessMessage"] = "Grade updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns();
            return View(model);
        }

        // GET: Admin/Grade/Delete/5
        public IActionResult Delete(int id)
        {
            // nếu view Delete đang nhận GradeListViewModel thì giữ kiểu cũ:
            var grades = _gradeService.GetAll();
            var grade = grades.FirstOrDefault(g => g.Id == id);
            if (grade == null) return NotFound();

            return View(grade);
        }

        // POST: Admin/Grade/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var grade = _db.Grades.FirstOrDefault(g => g.Id == id);
            if (grade == null)
            {
                TempData["ErrorMessage"] = "Grade not found!";
                return RedirectToAction(nameof(Index));
            }

            _db.Grades.Remove(grade);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Grade deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns()
        {
            // Approved enrollments chưa có grade
            // NOTE: cách này translate tốt trên SQL Server
            var enrollmentsWithoutGrades =
                (from e in _db.Enrollments.AsNoTracking()
                 where e.Status == EnrollmentStatus.Approved
                 where !_db.Grades.Any(g => g.EnrollmentId == e.Id)
                 join st in _db.Students.AsNoTracking() on e.StudentId equals st.Id into stj
                 from student in stj.DefaultIfEmpty()
                 join cc in _db.CourseClasses.AsNoTracking() on e.CourseClassId equals cc.Id into ccj
                 from courseClass in ccj.DefaultIfEmpty()
                 join sub in _db.Subjects.AsNoTracking() on (courseClass != null ? courseClass.SubjectId : 0) equals sub.Id into sj
                 from subject in sj.DefaultIfEmpty()
                 orderby student.StudentCode, courseClass.ClassCode
                 select new SelectListItem
                 {
                     Value = e.Id.ToString(),
                     Text = $"{(student != null ? student.StudentCode : "")} - {(student != null ? student.FullName : "")} | " +
                            $"{(courseClass != null ? courseClass.ClassCode : "")} - {(subject != null ? subject.SubjectName : "")}"
                 })
                .ToList();

            ViewBag.Enrollments = enrollmentsWithoutGrades;
        }
    }
}
