using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    public interface ILecturerStatisticsService
    {
        LecturerStatisticsViewModel GetStatistics(int lecturerId);
    }

    public class LecturerStatisticsService : ILecturerStatisticsService
    {
        private readonly ApplicationDbContext _db;

        public LecturerStatisticsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public LecturerStatisticsViewModel GetStatistics(int lecturerId)
        {
            var lecturer = _db.Lecturers.AsNoTracking().FirstOrDefault(l => l.Id == lecturerId);

            var classes = _db.CourseClasses
                .AsNoTracking()
                .Where(c => c.LecturerId == lecturerId)
                .ToList();

            if (classes.Count == 0)
            {
                return new LecturerStatisticsViewModel
                {
                    LecturerId = lecturerId,
                    LecturerName = lecturer?.FullName ?? string.Empty
                };
            }

            var classIds = classes.Select(c => c.Id).ToList();
            var subjectIds = classes.Select(c => c.SubjectId).Distinct().ToList();

            var subjects = _db.Subjects
                .AsNoTracking()
                .Where(s => subjectIds.Contains(s.Id))
                .ToDictionary(s => s.Id, s => s);

            var enrollments = _db.Enrollments
                .AsNoTracking()
                .Where(e => classIds.Contains(e.CourseClassId) && e.Status == EnrollmentStatus.Approved)
                .ToList();

            var studentIds = enrollments.Select(e => e.StudentId).Distinct().ToList();
            var students = _db.Students
                .AsNoTracking()
                .Where(s => studentIds.Contains(s.Id))
                .ToDictionary(s => s.Id, s => s);

            var grades = _db.Grades
                .AsNoTracking()
                .Where(g => classIds.Contains(g.CourseClassId))
                .ToList();

            var attendances = _db.Attendances
                .AsNoTracking()
                .Where(a => classIds.Contains(a.CourseClassId))
                .ToList();

            var model = new LecturerStatisticsViewModel
            {
                LecturerId = lecturerId,
                LecturerName = lecturer?.FullName ?? string.Empty,
                TotalClasses = classes.Count,
                TotalStudents = enrollments.Select(e => e.StudentId).Distinct().Count()
            };

            foreach (var courseClass in classes)
            {
                subjects.TryGetValue(courseClass.SubjectId, out var subject);

                var classEnrollments = enrollments
                    .Where(e => e.CourseClassId == courseClass.Id)
                    .ToList();

                var classGrades = grades
                    .Where(g => g.CourseClassId == courseClass.Id)
                    .ToList();

                var scoredGrades = classGrades
                    .Where(g => g.TotalScore.HasValue)
                    .Select(g => g.TotalScore!.Value)
                    .ToList();

                var averageScore = scoredGrades.Count > 0
                    ? Math.Round(scoredGrades.Average(), 2)
                    : 0;

                var excellent = scoredGrades.Count(s => s >= 9.0);
                var good = scoredGrades.Count(s => s >= 8.0 && s < 9.0);
                var fair = scoredGrades.Count(s => s >= 7.0 && s < 8.0);
                var average = scoredGrades.Count(s => s >= 5.0 && s < 7.0);
                var weak = scoredGrades.Count(s => s < 5.0);

                model.GradeStats.Add(new ClassGradeStatViewModel
                {
                    CourseClassId = courseClass.Id,
                    ClassCode = courseClass.ClassCode,
                    SubjectName = subject?.SubjectName ?? string.Empty,
                    StudentCount = classEnrollments.Count,
                    AverageScore = averageScore,
                    ExcellentCount = excellent,
                    GoodCount = good,
                    FairCount = fair,
                    AverageCount = average,
                    WeakCount = weak
                });

                var noScores = classEnrollments
                    .Where(e =>
                    {
                        var grade = classGrades.FirstOrDefault(g => g.EnrollmentId == e.Id);
                        return grade == null || !grade.TotalScore.HasValue;
                    })
                    .ToList();

                foreach (var enrollment in noScores)
                {
                    if (!students.TryGetValue(enrollment.StudentId, out var st)) continue;

                    model.StudentsWithoutScores.Add(new StudentNoScoreViewModel
                    {
                        StudentId = st.Id,
                        StudentCode = st.StudentCode,
                        StudentName = st.FullName,
                        ClassCode = courseClass.ClassCode,
                        SubjectName = subject?.SubjectName ?? string.Empty
                    });
                }

                var classAttendance = attendances
                    .Where(a => a.CourseClassId == courseClass.Id)
                    .ToList();

                var sessionGroups = classAttendance
                    .GroupBy(a => new { Date = a.AttendanceDate.Date, a.Session })
                    .ToList();

                var totalSessions = sessionGroups.Count;
                var studentCount = classEnrollments.Count;

                double avgAttendanceRate = 0;
                if (totalSessions > 0 && studentCount > 0)
                {
                    var totalPossible = totalSessions * studentCount;
                    var totalPresent = classAttendance.Count(a => a.Status == AttendanceStatus.Present);
                    avgAttendanceRate = Math.Round(totalPresent * 100.0 / totalPossible, 2);
                }

                var sessionDetails = sessionGroups
                    .Select(g =>
                    {
                        var present = g.Count(a => a.Status == AttendanceStatus.Present);
                        var absent = studentCount - present;
                        var rate = studentCount > 0 ? Math.Round(present * 100.0 / studentCount, 2) : 0;

                        return new SessionAttendanceDetailViewModel
                        {
                            SessionDate = g.Key.Date,
                            Session = g.Key.Session,
                            PresentCount = present,
                            AbsentCount = absent,
                            AttendanceRate = rate
                        };
                    })
                    .OrderBy(x => x.SessionDate)
                    .ThenBy(x => x.Session)
                    .ToList();

                var highAbsenceStudents = new List<AttendanceAlertViewModel>();

                foreach (var enrollment in classEnrollments)
                {
                    if (!students.TryGetValue(enrollment.StudentId, out var st)) continue;

                    var studentAttendances = classAttendance.Where(a => a.StudentId == enrollment.StudentId).ToList();
                    var present = studentAttendances.Count(a => a.Status == AttendanceStatus.Present);
                    var absent = totalSessions - present;
                    var rate = totalSessions > 0 ? Math.Round(present * 100.0 / totalSessions, 2) : 0;

                    if (totalSessions > 0 && rate < 80)
                    {
                        highAbsenceStudents.Add(new AttendanceAlertViewModel
                        {
                            StudentId = st.Id,
                            StudentCode = st.StudentCode,
                            StudentName = st.FullName,
                            PresentSessions = present,
                            AbsentSessions = absent,
                            AttendanceRate = rate
                        });
                    }
                }

                model.AttendanceStats.Add(new ClassAttendanceStatViewModel
                {
                    CourseClassId = courseClass.Id,
                    ClassCode = courseClass.ClassCode,
                    SubjectName = subject?.SubjectName ?? string.Empty,
                    StudentCount = studentCount,
                    AverageAttendanceRate = avgAttendanceRate,
                    HighAbsenceCount = highAbsenceStudents.Count,
                    HighAbsenceStudents = highAbsenceStudents
                        .OrderBy(x => x.AttendanceRate)
                        .ThenBy(x => x.StudentCode)
                        .ToList(),
                    SessionDetails = sessionDetails
                });
            }

            return model;
        }
    }
}
