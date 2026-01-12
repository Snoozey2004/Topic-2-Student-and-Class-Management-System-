using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service quản lý điểm danh (SQL Server + EF Core)
    /// </summary>
    public interface IAttendanceService
    {
        List<AttendanceClassListViewModel> GetClassesByLecturerId(int lecturerId, string? semester = null);
        AttendanceSessionViewModel GetAttendanceSession(int courseClassId, DateTime date, string session);
        bool TakeAttendance(TakeAttendanceViewModel model, int lecturerId);
        AttendanceHistoryViewModel GetAttendanceHistory(int courseClassId);
        bool UpdateAttendanceScore(int courseClassId);
    }

    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _db;
        private readonly IGradeService _gradeService;
        private readonly INotificationService _notificationService;

        public AttendanceService(ApplicationDbContext db, IGradeService gradeService, INotificationService notificationService)
        {
            _db = db;
            _gradeService = gradeService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Get classes for lecturer - semester passed from Controller
        /// </summary>
        public List<AttendanceClassListViewModel> GetClassesByLecturerId(int lecturerId, string? semester = null)
        {
            var classQuery = _db.CourseClasses
                .AsNoTracking()
                .Where(c => c.LecturerId == lecturerId);

            if (!string.IsNullOrWhiteSpace(semester))
            {
                var sem = semester.Trim();
                classQuery = classQuery.Where(c => c.Semester == sem);
            }

            var classes = classQuery.ToList();
            if (classes.Count == 0) return new List<AttendanceClassListViewModel>();

            var classIds = classes.Select(c => c.Id).ToList();

            var enrollments = _db.Enrollments
                .AsNoTracking()
                .Where(e => classIds.Contains(e.CourseClassId) && e.Status == EnrollmentStatus.Approved)
                .ToList();

            var attendances = _db.Attendances
                .AsNoTracking()
                .Where(a => classIds.Contains(a.CourseClassId))
                .ToList();

            var subjectIds = classes.Select(c => c.SubjectId).Distinct().ToList();
            var subjects = _db.Subjects
                .AsNoTracking()
                .Where(s => subjectIds.Contains(s.Id))
                .ToDictionary(s => s.Id, s => s);

            var result = classes.Select(c =>
            {
                subjects.TryGetValue(c.SubjectId, out var subject);

                var classEnrollments = enrollments
                    .Where(e => e.CourseClassId == c.Id)
                    .ToList();

                var classAttendances = attendances
                    .Where(a => a.CourseClassId == c.Id)
                    .ToList();

                var sessions = classAttendances
                    .GroupBy(a => new { Date = a.AttendanceDate.Date, a.Session })
                    .Count();

                var avgRate = sessions > 0 && classEnrollments.Count > 0
                    ? (classAttendances.Count(a => a.Status == AttendanceStatus.Present) * 100.0) / (sessions * classEnrollments.Count)
                    : 0;

                return new AttendanceClassListViewModel
                {
                    Id = c.Id,
                    ClassCode = c.ClassCode,
                    SubjectName = subject?.SubjectName ?? "",
                    Semester = c.Semester,
                    Room = c.Room,
                    TotalStudents = classEnrollments.Count,
                    TotalSessions = sessions,
                    AverageAttendanceRate = avgRate
                };
            })
            .OrderBy(x => x.ClassCode)
            .ToList();

            return result;
        }

        public AttendanceSessionViewModel GetAttendanceSession(int courseClassId, DateTime date, string session)
        {
            var courseClass = _db.CourseClasses
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == courseClassId);

            if (courseClass == null)
                return new AttendanceSessionViewModel();

            var subject = _db.Subjects
                .AsNoTracking()
                .FirstOrDefault(s => s.Id == courseClass.SubjectId);

            var enrollments = _db.Enrollments
                .AsNoTracking()
                .Where(e => e.CourseClassId == courseClassId && e.Status == EnrollmentStatus.Approved)
                .OrderBy(e => e.StudentId)
                .ToList();

            var studentIds = enrollments.Select(e => e.StudentId).Distinct().ToList();
            var students = _db.Students
                .AsNoTracking()
                .Where(s => studentIds.Contains(s.Id))
                .ToDictionary(s => s.Id, s => s);

            var existingAttendances = _db.Attendances
                .AsNoTracking()
                .Where(a => a.CourseClassId == courseClassId
                         && a.AttendanceDate.Date == date.Date
                         && a.Session == session)
                .ToList();

            var attendanceByStudent = existingAttendances
                .GroupBy(a => a.StudentId)
                .ToDictionary(g => g.Key, g => g.First());

            var list = enrollments.Select(e =>
            {
                students.TryGetValue(e.StudentId, out var st);
                attendanceByStudent.TryGetValue(e.StudentId, out var atd);

                return new StudentAttendanceViewModel
                {
                    StudentId = e.StudentId,
                    EnrollmentId = e.Id,
                    StudentCode = st?.StudentCode ?? "",
                    FullName = st?.FullName ?? "",
                    Email = st?.Email ?? "",
                    // Mặc định Present nếu chưa có record
                    IsPresent = atd == null || atd.Status == AttendanceStatus.Present,
                    Status = atd?.Status ?? AttendanceStatus.Present,
                    Note = atd?.Note
                };
            })
            .OrderBy(x => x.StudentCode)
            .ToList();

            return new AttendanceSessionViewModel
            {
                CourseClassId = courseClassId,
                ClassCode = courseClass.ClassCode,
                SubjectName = subject?.SubjectName ?? "",
                SessionDate = date,
                Session = session,
                Room = courseClass.Room,
                Students = list
            };
        }

        public bool TakeAttendance(TakeAttendanceViewModel model, int lecturerId)
        {
            var ownsClass = _db.CourseClasses
                .AsNoTracking()
                .Any(c => c.Id == model.CourseClassId && c.LecturerId == lecturerId);

            if (!ownsClass) return false;

            // Normalize to date-only to avoid timezone/Kind related day shifting
            model.SessionDate = DateTime.SpecifyKind(model.SessionDate.Date, DateTimeKind.Unspecified);

            var day = model.SessionDate.DayOfWeek;
            var session = (model.Session ?? string.Empty).Trim();

            // If attendance already exists for this session, allow updating it (debug/edit)
            var alreadyExists = _db.Attendances
                .AsNoTracking()
                .Any(a =>
                    a.CourseClassId == model.CourseClassId &&
                    a.AttendanceDate.Date == model.SessionDate.Date &&
                    a.Session == session);

            if (!alreadyExists)
            {
                // For new sessions, require schedule match
                var hasScheduleToday = _db.Schedules
                    .AsNoTracking()
                    .Any(s =>
                        s.CourseClassId == model.CourseClassId &&
                        s.DayOfWeek == day &&
                        s.EffectiveDate.Date <= model.SessionDate.Date &&
                        (s.EndDate == null || s.EndDate.Value.Date >= model.SessionDate.Date) &&
                        s.Session == session);

                if (!hasScheduleToday) return false;
            }

            using var tx = _db.Database.BeginTransaction();

            try
            {
                var oldAttendances = _db.Attendances
                    .Where(a => a.CourseClassId == model.CourseClassId
                             && a.AttendanceDate.Date == model.SessionDate.Date
                             && a.Session == model.Session)
                    .ToList();

                if (oldAttendances.Count > 0)
                {
                    _db.Attendances.RemoveRange(oldAttendances);
                    _db.SaveChanges();
                }

                var now = DateTime.Now;

                foreach (var student in model.Students)
                {
                    _db.Attendances.Add(new Attendance
                    {
                        EnrollmentId = student.EnrollmentId,
                        StudentId = student.StudentId,
                        CourseClassId = model.CourseClassId,
                        AttendanceDate = model.SessionDate,
                        Session = session,
                        Status = student.IsPresent ? AttendanceStatus.Present : AttendanceStatus.Absent,
                        Note = student.Note,
                        CreatedDate = now,
                        CreatedBy = lecturerId
                    });
                }

                _db.SaveChanges();

                CreateAbsentNotifications(model);

                var ok = UpdateAttendanceScore(model.CourseClassId);
                if (!ok)
                {
                    tx.Rollback();
                    return false;
                }

                tx.Commit();
                return true;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        private void CreateAbsentNotifications(TakeAttendanceViewModel model)
        {
            var absentStudentIds = model.Students
                .Where(s => !s.IsPresent)
                .Select(s => s.StudentId)
                .Distinct()
                .ToList();

            if (absentStudentIds.Count == 0) return;

            var courseClass = _db.CourseClasses
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == model.CourseClassId);

            if (courseClass == null) return;

            var subjectName = _db.Subjects
                .AsNoTracking()
                .Where(s => s.Id == courseClass.SubjectId)
                .Select(s => s.SubjectName)
                .FirstOrDefault() ?? string.Empty;

            // map StudentId -> UserId
            var userIds = _db.Students
                .AsNoTracking()
                .Where(s => absentStudentIds.Contains(s.Id))
                .Select(s => new { s.Id, s.UserId })
                .ToList();

            // Idempotency: remove previous missed-class notifications for the same class/date/session
            // (TakeAttendance can be re-submitted and it rebuilds attendance rows)
            var title = $"Missed class: {courseClass.ClassCode}";
            var dateLabel = model.SessionDate.Date.ToString("dd/MM/yyyy");
            var message = $"You were marked absent for {courseClass.ClassCode} ({subjectName}) on {dateLabel} ({model.Session}).";

            var linkUrl = "/Student/Notification/Attendance";

            var userIdList = userIds.Select(x => x.UserId).Distinct().ToList();
            var existing = _db.Notifications
                .Where(n => userIdList.Contains(n.UserId)
                         && n.Type == NotificationType.Attendance
                         && n.LinkUrl == linkUrl
                         && n.Title == title
                         && n.Message == message)
                .ToList();

            if (existing.Count > 0)
            {
                _db.Notifications.RemoveRange(existing);
                _db.SaveChanges();
            }

            foreach (var u in userIds)
            {
                if (u.UserId == 0) continue;

                _notificationService.CreateNotification(
                    u.UserId,
                    title,
                    message,
                    NotificationType.Attendance,
                    linkUrl
                );
            }
        }

        public AttendanceHistoryViewModel GetAttendanceHistory(int courseClassId)
        {
            var courseClass = _db.CourseClasses.AsNoTracking().FirstOrDefault(c => c.Id == courseClassId);
            if (courseClass == null)
                return new AttendanceHistoryViewModel();

            var subject = _db.Subjects.AsNoTracking().FirstOrDefault(s => s.Id == courseClass.SubjectId);

            var enrollments = _db.Enrollments
                .AsNoTracking()
                .Where(e => e.CourseClassId == courseClassId && e.Status == EnrollmentStatus.Approved)
                .ToList();

            var attendances = _db.Attendances
                .AsNoTracking()
                .Where(a => a.CourseClassId == courseClassId)
                .OrderBy(a => a.AttendanceDate)
                .ThenBy(a => a.Session)
                .ToList();

            var records = attendances
                .GroupBy(a => new { Date = a.AttendanceDate.Date, a.Session })
                .Select(g =>
                {
                    var totalStudents = enrollments.Count;
                    var presentCount = g.Count(a => a.Status == AttendanceStatus.Present);
                    var absentCount = totalStudents - presentCount;

                    return new AttendanceRecordViewModel
                    {
                        SessionDate = g.Key.Date,
                        Session = g.Key.Session,
                        TotalStudents = totalStudents,
                        PresentCount = presentCount,
                        AbsentCount = absentCount,
                        AttendanceRate = totalStudents > 0 ? (presentCount * 100.0 / totalStudents) : 0
                    };
                })
                .ToList();

            var studentIds = enrollments.Select(e => e.StudentId).Distinct().ToList();
            var students = _db.Students
                .AsNoTracking()
                .Where(s => studentIds.Contains(s.Id))
                .ToDictionary(s => s.Id, s => s);

            var studentStats = new Dictionary<int, AttendanceStatViewModel>();
            var totalSessions = records.Count;

            foreach (var enrollment in enrollments)
            {
                students.TryGetValue(enrollment.StudentId, out var st);
                if (st == null) continue;

                var studentAttendances = attendances.Where(a => a.StudentId == enrollment.StudentId).ToList();
                var presentSessions = studentAttendances.Count(a => a.Status == AttendanceStatus.Present);
                var absentSessions = totalSessions - presentSessions;
                var rate = totalSessions > 0 ? (presentSessions * 100.0 / totalSessions) : 0;

                // giống code cũ: score = rate/10 (0-10)
                var score = Math.Round(rate / 10.0, 1);

                studentStats[enrollment.StudentId] = new AttendanceStatViewModel
                {
                    StudentId = enrollment.StudentId,
                    StudentCode = st.StudentCode,
                    FullName = st.FullName,
                    TotalSessions = totalSessions,
                    PresentSessions = presentSessions,
                    AbsentSessions = absentSessions,
                    AttendanceRate = rate,
                    AttendanceScore = score
                };
            }

            return new AttendanceHistoryViewModel
            {
                CourseClassId = courseClassId,
                ClassCode = courseClass.ClassCode,
                SubjectName = subject?.SubjectName ?? "",
                Records = records,
                StudentStats = studentStats
            };
        }

        public bool UpdateAttendanceScore(int courseClassId)
        {
            var history = GetAttendanceHistory(courseClassId);

            // Lấy tất cả enrollment của lớp
            var enrollments = _db.Enrollments
                .Where(e => e.CourseClassId == courseClassId && e.Status == EnrollmentStatus.Approved)
                .ToList();

            if (enrollments.Count == 0) return true;

            var enrollmentIds = enrollments.Select(e => e.Id).ToList();

            // Lấy grade theo enrollmentId (1-1)
            var grades = _db.Grades
                .Where(g => enrollmentIds.Contains(g.EnrollmentId))
                .ToDictionary(g => g.EnrollmentId, g => g);

            var now = DateTime.Now;

            foreach (var stat in history.StudentStats.Values)
            {
                var enrollment = enrollments.FirstOrDefault(e => e.StudentId == stat.StudentId);
                if (enrollment == null) continue;

                if (!grades.TryGetValue(enrollment.Id, out var grade) || grade == null)
                    continue; // nếu muốn auto tạo grade ở đây thì nói tao chỉnh

                grade.AttendanceScore = stat.AttendanceScore;
                grade.LastUpdated = now;

                // Giữ đúng logic cũ: chỉ tính total khi có Midterm + Final
                if (grade.MidtermScore.HasValue && grade.FinalScore.HasValue)
                {
                    var totalScore =
                        (grade.AttendanceScore ?? 0) * 0.1 +
                        grade.MidtermScore.Value * 0.3 +
                        grade.FinalScore.Value * 0.6;

                    grade.TotalScore = Math.Round(totalScore, 2);
                    grade.LetterGrade = _gradeService.CalculateLetterGrade(totalScore);
                    grade.IsPassed = totalScore >= 4.0;
                }
            }

            _db.SaveChanges();
            return true;
        }
    }
}
