using System.Linq;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service quản lý Grade
    /// </summary>
    public interface IGradeService
    {
        List<GradeInputViewModel> GetGradesByCourseClass(int courseClassId);
        List<WebApplication1.ViewModels.GradeListViewModel> GetAll();
        StudentGradeViewModel? GetStudentGrades(int studentId, string? semester = null);
        Grade? GetByEnrollmentId(int enrollmentId);
        bool UpdateGrade(GradeInputViewModel model, int lecturerId);
        bool BatchUpdateGrades(List<GradeInputViewModel> grades, int lecturerId);
        string CalculateLetterGrade(double totalScore);
    }

    public class GradeService : IGradeService
    {
        private readonly INotificationService _notificationService;

        public GradeService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public List<WebApplication1.ViewModels.GradeListViewModel> GetAll()
        {
            return FakeDatabase.Grades.Select(g =>
            {
                var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == g.StudentId);
                var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == g.CourseClassId);

                return new WebApplication1.ViewModels.GradeListViewModel
                {
                    Id = g.Id,
                    StudentName = student?.FullName ?? string.Empty,
                    CourseClassCode = courseClass?.ClassCode ?? string.Empty,
                    TotalScore = g.TotalScore,
                    LetterGrade = g.LetterGrade
                };
            }).ToList();
        }

        public List<GradeInputViewModel> GetGradesByCourseClass(int courseClassId)
        {
            var enrollments = FakeDatabase.Enrollments
                .Where(e => e.CourseClassId == courseClassId && e.Status == EnrollmentStatus.Approved)
                .ToList();

            return enrollments.Select(e =>
            {
                var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == e.StudentId);
                var grade = FakeDatabase.Grades.FirstOrDefault(g => g.EnrollmentId == e.Id);

                return new GradeInputViewModel
                {
                    GradeId = grade?.Id ?? 0,
                    EnrollmentId = e.Id,
                    StudentId = e.StudentId,
                    StudentCode = student?.StudentCode ?? "",
                    StudentName = student?.FullName ?? "",
                    AttendanceScore = grade?.AttendanceScore,
                    MidtermScore = grade?.MidtermScore,
                    FinalScore = grade?.FinalScore
                };
            }).OrderBy(g => g.StudentCode).ToList();
        }

        public StudentGradeViewModel? GetStudentGrades(int studentId, string? semester = null)
        {
            var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == studentId);
            if (student == null) return null;

            var enrollments = FakeDatabase.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved)
                .ToList();

            if (!string.IsNullOrEmpty(semester))
            {
                var courseClassIds = FakeDatabase.CourseClasses
                    .Where(c => c.Semester == semester)
                    .Select(c => c.Id)
                    .ToList();
                enrollments = enrollments.Where(e => courseClassIds.Contains(e.CourseClassId)).ToList();
            }

            var grades = enrollments.Select(e =>
            {
                var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == e.CourseClassId);
                var subject = courseClass != null
                    ? FakeDatabase.Subjects.FirstOrDefault(s => s.Id == courseClass.SubjectId)
                    : null;
                var grade = FakeDatabase.Grades.FirstOrDefault(g => g.EnrollmentId == e.Id);

                return new GradeDetailViewModel
                {
                    SubjectCode = subject?.SubjectCode ?? "",
                    SubjectName = subject?.SubjectName ?? "",
                    Credits = subject?.Credits ?? 0,
                    ClassCode = courseClass?.ClassCode ?? "",
                    AttendanceScore = grade?.AttendanceScore,
                    MidtermScore = grade?.MidtermScore,
                    FinalScore = grade?.FinalScore,
                    TotalScore = grade?.TotalScore,
                    LetterGrade = grade?.LetterGrade,
                    IsPassed = grade?.IsPassed ?? false
                };
            }).ToList();

            // Tính GPA
            double totalPoints = 0;
            int totalCredits = 0;
            int passedCredits = 0;

            foreach (var grade in grades.Where(g => g.TotalScore.HasValue))
            {
                totalPoints += grade.TotalScore!.Value * grade.Credits;
                totalCredits += grade.Credits;
                if (grade.IsPassed)
                {
                    passedCredits += grade.Credits;
                }
            }

            double? gpa = totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : null;

            return new StudentGradeViewModel
            {
                StudentCode = student.StudentCode,
                StudentName = student.FullName,
                Semester = semester ?? "Tất cả",
                Grades = grades,
                GPA = gpa,
                TotalCredits = totalCredits,
                PassedCredits = passedCredits
            };
        }

        public Grade? GetByEnrollmentId(int enrollmentId)
        {
            return FakeDatabase.Grades.FirstOrDefault(g => g.EnrollmentId == enrollmentId);
        }

        public bool UpdateGrade(GradeInputViewModel model, int lecturerId)
        {
            var grade = FakeDatabase.Grades.FirstOrDefault(g => g.Id == model.GradeId);
            if (grade == null) return false;

            grade.AttendanceScore = model.AttendanceScore;
            grade.MidtermScore = model.MidtermScore;
            grade.FinalScore = model.FinalScore;

            // Tính điểm tổng kết nếu đủ điểm thành phần
            if (grade.AttendanceScore.HasValue && grade.MidtermScore.HasValue && grade.FinalScore.HasValue)
            {
                grade.TotalScore = Math.Round(
                    grade.AttendanceScore.Value * 0.1 +
                    grade.MidtermScore.Value * 0.3 +
                    grade.FinalScore.Value * 0.6,
                    2);

                grade.LetterGrade = CalculateLetterGrade(grade.TotalScore.Value);
                grade.IsPassed = grade.TotalScore.Value >= 4.0;
            }

            grade.LastUpdated = DateTime.Now;
            grade.UpdatedBy = lecturerId;

            // Tạo thông báo cho sinh viên nếu có điểm mới
            if (grade.TotalScore.HasValue)
            {
                var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == grade.StudentId);
                if (student != null)
                {
                    _notificationService.CreateNotification(
                        student.UserId,
                        "Có điểm mới",
                        "Điểm số của bạn đã được cập nhật. Vui lòng kiểm tra bảng điểm.",
                        NotificationType.Grade,
                        "/Student/Grades"
                    );
                }
            }

            return true;
        }

        public bool BatchUpdateGrades(List<GradeInputViewModel> grades, int lecturerId)
        {
            foreach (var gradeModel in grades)
            {
                UpdateGrade(gradeModel, lecturerId);
            }
            return true;
        }

        public string CalculateLetterGrade(double totalScore)
        {
            if (totalScore >= 8.5) return "A";
            if (totalScore >= 8.0) return "B+";
            if (totalScore >= 7.0) return "B";
            if (totalScore >= 6.5) return "C+";
            if (totalScore >= 5.5) return "C";
            if (totalScore >= 5.0) return "D+";
            if (totalScore >= 4.0) return "D";
            return "F";
        }
    }
}
