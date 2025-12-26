using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service qu?n lý ?i?m danh
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
        private readonly ICourseClassService _courseClassService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentService _studentService;
        private readonly ISubjectService _subjectService;
        private readonly IGradeService _gradeService;

        public AttendanceService(
            ICourseClassService courseClassService,
            IEnrollmentService enrollmentService,
            IStudentService studentService,
            ISubjectService subjectService,
            IGradeService gradeService)
        {
            _courseClassService = courseClassService;
            _enrollmentService = enrollmentService;
            _studentService = studentService;
            _subjectService = subjectService;
            _gradeService = gradeService;
        }

        /// <summary>
        /// Get classes for lecturer - semester passed from Controller
        /// </summary>
        public List<AttendanceClassListViewModel> GetClassesByLecturerId(int lecturerId, string? semester = null)
        {
            var query = FakeDatabase.CourseClasses
                .Where(c => c.LecturerId == lecturerId);

            // Filter by semester if provided
            if (!string.IsNullOrEmpty(semester))
            {
                query = query.Where(c => c.Semester == semester);
            }

            var classes = query.Select(c =>
            {
                var subject = _subjectService.GetById(c.SubjectId);
                var enrollments = FakeDatabase.Enrollments
                    .Where(e => e.CourseClassId == c.Id && e.Status == EnrollmentStatus.Approved)
                    .ToList();
                var attendances = FakeDatabase.Attendances.Where(a => a.CourseClassId == c.Id).ToList();

                var sessions = attendances.GroupBy(a => new { a.AttendanceDate, a.Session }).Count();

                var avgRate = sessions > 0 && enrollments.Count > 0
                    ? (attendances.Count(a => a.Status == AttendanceStatus.Present) * 100.0) / (sessions * enrollments.Count)
                    : 0;

                return new AttendanceClassListViewModel
                {
                    Id = c.Id,
                    ClassCode = c.ClassCode,
                    SubjectName = subject?.SubjectName ?? "",
                    Semester = c.Semester,
                    Room = c.Room,
                    TotalStudents = enrollments.Count,
                    TotalSessions = sessions,
                    AverageAttendanceRate = avgRate
                };
            })
            .OrderBy(c => c.ClassCode)
            .ToList();

            return classes;
        }

        public AttendanceSessionViewModel GetAttendanceSession(int courseClassId, DateTime date, string session)
        {
            var courseClass = _courseClassService.GetById(courseClassId);
            if (courseClass == null)
                return new AttendanceSessionViewModel();

            var subject = _subjectService.GetById(courseClass.SubjectId);
            var enrollments = FakeDatabase.Enrollments
                .Where(e => e.CourseClassId == courseClassId && e.Status == EnrollmentStatus.Approved)
                .ToList();

            var existingAttendances = FakeDatabase.Attendances
                .Where(a => a.CourseClassId == courseClassId 
                         && a.AttendanceDate.Date == date.Date 
                         && a.Session == session)
                .ToList();

            var students = enrollments.Select(e =>
            {
                var student = _studentService.GetById(e.StudentId);
                var attendance = existingAttendances.FirstOrDefault(a => a.StudentId == e.StudentId);

                return new StudentAttendanceViewModel
                {
                    StudentId = e.StudentId,
                    EnrollmentId = e.Id,
                    StudentCode = student?.StudentCode ?? "",
                    FullName = student?.FullName ?? "",
                    Email = student?.Email ?? "",
                    IsPresent = attendance?.Status == AttendanceStatus.Present || attendance == null,
                    Status = attendance?.Status ?? AttendanceStatus.Present,
                    Note = attendance?.Note
                };
            }).OrderBy(s => s.StudentCode).ToList();

            return new AttendanceSessionViewModel
            {
                CourseClassId = courseClassId,
                ClassCode = courseClass.ClassCode,
                SubjectName = subject?.SubjectName ?? "",
                SessionDate = date,
                Session = session,
                Room = courseClass.Room,
                Students = students
            };
        }

        public bool TakeAttendance(TakeAttendanceViewModel model, int lecturerId)
        {
            var oldAttendances = FakeDatabase.Attendances
                .Where(a => a.CourseClassId == model.CourseClassId
                         && a.AttendanceDate.Date == model.SessionDate.Date
                         && a.Session == model.Session)
                .ToList();

            foreach (var old in oldAttendances)
            {
                FakeDatabase.Attendances.Remove(old);
            }

            foreach (var student in model.Students)
            {
                var attendance = new Attendance
                {
                    Id = FakeDatabase.GetNextAttendanceId(),
                    EnrollmentId = student.EnrollmentId,
                    StudentId = student.StudentId,
                    CourseClassId = model.CourseClassId,
                    AttendanceDate = model.SessionDate,
                    Session = model.Session,
                    Status = student.IsPresent ? AttendanceStatus.Present : AttendanceStatus.Absent,
                    Note = student.Note,
                    CreatedDate = DateTime.Now,
                    CreatedBy = lecturerId
                };

                FakeDatabase.Attendances.Add(attendance);
            }

            UpdateAttendanceScore(model.CourseClassId);

            return true;
        }

        public AttendanceHistoryViewModel GetAttendanceHistory(int courseClassId)
        {
            var courseClass = _courseClassService.GetById(courseClassId);
            if (courseClass == null)
                return new AttendanceHistoryViewModel();

            var subject = _subjectService.GetById(courseClass.SubjectId);
            var enrollments = FakeDatabase.Enrollments
                .Where(e => e.CourseClassId == courseClassId && e.Status == EnrollmentStatus.Approved)
                .ToList();
            var attendances = FakeDatabase.Attendances
                .Where(a => a.CourseClassId == courseClassId)
                .OrderBy(a => a.AttendanceDate)
                .ThenBy(a => a.Session)
                .ToList();

            var records = attendances
                .GroupBy(a => new { a.AttendanceDate, a.Session })
                .Select(g =>
                {
                    var totalStudents = enrollments.Count;
                    var presentCount = g.Count(a => a.Status == AttendanceStatus.Present);
                    var absentCount = totalStudents - presentCount;

                    return new AttendanceRecordViewModel
                    {
                        SessionDate = g.Key.AttendanceDate,
                        Session = g.Key.Session,
                        TotalStudents = totalStudents,
                        PresentCount = presentCount,
                        AbsentCount = absentCount,
                        AttendanceRate = totalStudents > 0 ? (presentCount * 100.0 / totalStudents) : 0
                    };
                })
                .ToList();

            var studentStats = new Dictionary<int, AttendanceStatViewModel>();

            foreach (var enrollment in enrollments)
            {
                var student = _studentService.GetById(enrollment.StudentId);
                if (student == null) continue;

                var studentAttendances = attendances.Where(a => a.StudentId == enrollment.StudentId).ToList();
                var totalSessions = records.Count;
                var presentSessions = studentAttendances.Count(a => a.Status == AttendanceStatus.Present);
                var absentSessions = totalSessions - presentSessions;
                var rate = totalSessions > 0 ? (presentSessions * 100.0 / totalSessions) : 0;

                var score = Math.Round(rate / 10.0, 1);

                studentStats[enrollment.StudentId] = new AttendanceStatViewModel
                {
                    StudentId = enrollment.StudentId,
                    StudentCode = student.StudentCode,
                    FullName = student.FullName,
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

            foreach (var stat in history.StudentStats.Values)
            {
                var enrollment = FakeDatabase.Enrollments
                    .FirstOrDefault(e => e.StudentId == stat.StudentId && e.CourseClassId == courseClassId);

                if (enrollment == null) continue;

                var grade = FakeDatabase.Grades.FirstOrDefault(g => g.EnrollmentId == enrollment.Id);

                if (grade != null)
                {
                    grade.AttendanceScore = stat.AttendanceScore;
                    grade.LastUpdated = DateTime.Now;

                    if (grade.MidtermScore.HasValue && grade.FinalScore.HasValue)
                    {
                        var totalScore = (grade.AttendanceScore ?? 0) * 0.1 
                                       + grade.MidtermScore.Value * 0.3 
                                       + grade.FinalScore.Value * 0.6;
                        
                        grade.TotalScore = Math.Round(totalScore, 2);
                        grade.LetterGrade = _gradeService.CalculateLetterGrade(totalScore);
                        grade.IsPassed = totalScore >= 4.0;
                    }
                }
            }

            return true;
        }
    }
}
