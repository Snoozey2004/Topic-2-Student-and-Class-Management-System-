using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service quản lý Grade (SQL Server + EF Core)
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
        private readonly ApplicationDbContext _db;
        private readonly INotificationService _notificationService;

        public GradeService(ApplicationDbContext db, INotificationService notificationService)
        {
            _db = db;
            _notificationService = notificationService;
        }

        public List<WebApplication1.ViewModels.GradeListViewModel> GetAll()
        {
            var query =
                from g in _db.Grades
                join st in _db.Students on g.StudentId equals st.Id into stj
                from student in stj.DefaultIfEmpty()
                join cc in _db.CourseClasses on g.CourseClassId equals cc.Id into ccj
                from courseClass in ccj.DefaultIfEmpty()
                select new WebApplication1.ViewModels.GradeListViewModel
                {
                    Id = g.Id,
                    StudentName = student != null ? student.FullName : string.Empty,
                    CourseClassCode = courseClass != null ? courseClass.ClassCode : string.Empty,
                    TotalScore = g.TotalScore,
                    LetterGrade = g.LetterGrade
                };

            return query.AsNoTracking().ToList();
        }

        public List<GradeInputViewModel> GetGradesByCourseClass(int courseClassId)
        {
            // Lấy enrollments đã Approved trong lớp
            var enrollments = _db.Enrollments
                .AsNoTracking()
                .Where(e => e.CourseClassId == courseClassId && e.Status == EnrollmentStatus.Approved)
                .ToList();

            var enrollmentIds = enrollments.Select(e => e.Id).ToList();
            var studentIds = enrollments.Select(e => e.StudentId).Distinct().ToList();

            var students = _db.Students
                .AsNoTracking()
                .Where(s => studentIds.Contains(s.Id))
                .ToDictionary(s => s.Id, s => s);

            var gradeByEnrollment = _db.Grades
                .AsNoTracking()
                .Where(g => enrollmentIds.Contains(g.EnrollmentId))
                .ToDictionary(g => g.EnrollmentId, g => g);

            var result = enrollments.Select(e =>
            {
                students.TryGetValue(e.StudentId, out var student);
                gradeByEnrollment.TryGetValue(e.Id, out var grade);

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
            })
            .OrderBy(x => x.StudentCode)
            .ToList();

            return result;
        }

        public StudentGradeViewModel? GetStudentGrades(int studentId, string? semester = null)
        {
            var student = _db.Students
                .AsNoTracking()
                .FirstOrDefault(s => s.Id == studentId);

            if (student == null) return null;

            // enrollments approved
            var enrollmentsQuery = _db.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved);

            if (!string.IsNullOrWhiteSpace(semester))
            {
                // chỉ lấy những enrollment thuộc lớp có semester đó
                var classIdsInSemester = _db.CourseClasses
                    .AsNoTracking()
                    .Where(c => c.Semester == semester)
                    .Select(c => c.Id);

                enrollmentsQuery = enrollmentsQuery.Where(e => classIdsInSemester.Contains(e.CourseClassId));
            }

            var enrollments = enrollmentsQuery.ToList();
            var classIds = enrollments.Select(e => e.CourseClassId).Distinct().ToList();
            var enrollmentIds = enrollments.Select(e => e.Id).ToList();

            var courseClasses = _db.CourseClasses
                .AsNoTracking()
                .Where(c => classIds.Contains(c.Id))
                .ToDictionary(c => c.Id, c => c);

            var subjectIds = courseClasses.Values.Select(c => c.SubjectId).Distinct().ToList();
            var subjects = _db.Subjects
                .AsNoTracking()
                .Where(s => subjectIds.Contains(s.Id))
                .ToDictionary(s => s.Id, s => s);

            var grades = _db.Grades
                .AsNoTracking()
                .Where(g => enrollmentIds.Contains(g.EnrollmentId))
                .ToDictionary(g => g.EnrollmentId, g => g);

            var gradeDetails = enrollments.Select(e =>
            {
                courseClasses.TryGetValue(e.CourseClassId, out var cc);
                Subject? subject = null;
                if (cc != null) subjects.TryGetValue(cc.SubjectId, out subject);

                grades.TryGetValue(e.Id, out var grade);

                return new GradeDetailViewModel
                {
                    SubjectCode = subject?.SubjectCode ?? "",
                    SubjectName = subject?.SubjectName ?? "",
                    Credits = subject?.Credits ?? 0,
                    ClassCode = cc?.ClassCode ?? "",
                    AttendanceScore = grade?.AttendanceScore,
                    MidtermScore = grade?.MidtermScore,
                    FinalScore = grade?.FinalScore,
                    TotalScore = grade?.TotalScore,
                    LetterGrade = grade?.LetterGrade,
                    IsPassed = grade?.IsPassed ?? false
                };
            }).ToList();

            // GPA theo TotalScore * Credits (giữ nguyên logic mày đang làm)
            double totalPoints = 0;
            int totalCredits = 0;
            int passedCredits = 0;

            foreach (var gd in gradeDetails.Where(g => g.TotalScore.HasValue))
            {
                totalPoints += gd.TotalScore!.Value * gd.Credits;
                totalCredits += gd.Credits;

                if (gd.IsPassed)
                    passedCredits += gd.Credits;
            }

            double? gpa = totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : null;

            return new StudentGradeViewModel
            {
                StudentCode = student.StudentCode,
                StudentName = student.FullName,
                Semester = semester ?? "Tất cả",
                Grades = gradeDetails,
                GPA = gpa,
                TotalCredits = totalCredits,
                PassedCredits = passedCredits
            };
        }

        public Grade? GetByEnrollmentId(int enrollmentId)
        {
            return _db.Grades.FirstOrDefault(g => g.EnrollmentId == enrollmentId);
        }

        public bool UpdateGrade(GradeInputViewModel model, int lecturerId)
        {
            // Nếu có GradeId thì update; nếu chưa có thì tạo mới theo EnrollmentId
            Grade? grade = null;

            if (model.GradeId > 0)
            {
                grade = _db.Grades.FirstOrDefault(g => g.Id == model.GradeId);
            }

            if (grade == null)
            {
                // tìm grade theo enrollmentId (phòng trường hợp gradeId = 0)
                grade = _db.Grades.FirstOrDefault(g => g.EnrollmentId == model.EnrollmentId);

                if (grade == null)
                {
                    // tạo mới grade (cần lấy StudentId & CourseClassId từ Enrollment)
                    var enrollment = _db.Enrollments
                        .AsNoTracking()
                        .FirstOrDefault(e => e.Id == model.EnrollmentId);

                    if (enrollment == null) return false;

                    grade = new Grade
                    {
                        // KHÔNG set Id nếu Identity
                        EnrollmentId = enrollment.Id,
                        StudentId = enrollment.StudentId,
                        CourseClassId = enrollment.CourseClassId
                    };

                    _db.Grades.Add(grade);
                }
            }

            grade.AttendanceScore = model.AttendanceScore;
            grade.MidtermScore = model.MidtermScore;
            grade.FinalScore = model.FinalScore;

            // Tính tổng kết nếu đủ điểm
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
            else
            {
                // nếu chưa đủ điểm, mày có thể chọn clear TotalScore/LetterGrade
                // (đang giữ an toàn: không xóa)
            }

            grade.LastUpdated = DateTime.Now;
            grade.UpdatedBy = lecturerId;

            _db.SaveChanges();

            // Notify student nếu đã có TotalScore
            if (grade.TotalScore.HasValue)
            {
                var studentUserId = _db.Students
                    .AsNoTracking()
                    .Where(s => s.Id == grade.StudentId)
                    .Select(s => s.UserId)
                    .FirstOrDefault();

                if (studentUserId != 0)
                {
                    _notificationService.CreateNotification(
                        studentUserId,
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
                var ok = UpdateGrade(gradeModel, lecturerId);
                if (!ok) return false;
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
