using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    public interface IStudentStatisticsService
    {
        StudentStatisticsViewModel GetStatistics(int studentId);
    }

    public class StudentStatisticsService : IStudentStatisticsService
    {
        private readonly ApplicationDbContext _db;

        public StudentStatisticsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public StudentStatisticsViewModel GetStatistics(int studentId)
        {
            var student = _db.Students.AsNoTracking().FirstOrDefault(s => s.Id == studentId);
            if (student == null)
            {
                return new StudentStatisticsViewModel();
            }

            var enrollments = _db.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved)
                .ToList();

            var classIds = enrollments.Select(e => e.CourseClassId).Distinct().ToList();
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
                .Where(g => classIds.Contains(g.CourseClassId) && enrollments.Select(e => e.Id).Contains(g.EnrollmentId))
                .ToDictionary(g => g.EnrollmentId, g => g);

            var attendances = _db.Attendances
                .AsNoTracking()
                .Where(a => a.StudentId == studentId && classIds.Contains(a.CourseClassId))
                .ToList();

            var model = new StudentStatisticsViewModel
            {
                StudentId = student.Id,
                StudentName = student.FullName,
                CurrentCourseCount = enrollments.Count,
                TotalRegisteredCredits = enrollments.Sum(e =>
                {
                    if (!courseClasses.TryGetValue(e.CourseClassId, out var cc)) return 0;
                    return subjects.TryGetValue(cc.SubjectId, out var sb) ? sb.Credits : 0;
                })
            };

            model.ThisWeekSchedule = BuildThisWeekSchedule(courseClasses, subjects);

            model.CourseGrades = BuildCourseGrades(enrollments, courseClasses, subjects, grades);
            model.GPA = CalculateGpa(model.CourseGrades);

            var gradeByCourseClass = enrollments
                .ToDictionary(e => e.CourseClassId, e => grades.TryGetValue(e.Id, out var g) ? g : null);

            model.PassedCount = model.CourseGrades.Count(g =>
            {
                if (gradeByCourseClass.TryGetValue(g.CourseClassId, out var gr) && gr != null && gr.TotalScore.HasValue)
                {
                    return gr.IsPassed || gr.TotalScore.Value >= 4.0;
                }
                return g.TotalScore.HasValue && g.TotalScore.Value >= 4.0;
            });

            model.FailedCount = model.CourseGrades.Count(g =>
            {
                if (gradeByCourseClass.TryGetValue(g.CourseClassId, out var gr) && gr != null && gr.TotalScore.HasValue)
                {
                    return gr.TotalScore.Value < 4.0 && !gr.IsPassed;
                }
                return g.TotalScore.HasValue && g.TotalScore.Value < 4.0;
            });

            model.AttendanceSummary = BuildAttendanceSummary(enrollments, courseClasses, subjects, attendances);

            return model;
        }

        private List<WeeklyScheduleItemViewModel> BuildThisWeekSchedule(
            Dictionary<int, CourseClass> courseClasses,
            Dictionary<int, Subject> subjects)
        {
            var today = DateTime.Today;
            var diff = (7 + (int)today.DayOfWeek - (int)DayOfWeek.Monday) % 7;
            var weekStart = today.AddDays(-diff);
            var weekEnd = weekStart.AddDays(6);

            var schedules = _db.Schedules
                .AsNoTracking()
                .Where(s => courseClasses.Keys.Contains(s.CourseClassId))
                .ToList();

            var result = new List<WeeklyScheduleItemViewModel>();

            for (var date = weekStart; date <= weekEnd; date = date.AddDays(1))
            {
                var dayOfWeek = date.DayOfWeek;
                var daySchedules = schedules.Where(s => s.DayOfWeek == dayOfWeek && s.EffectiveDate.Date <= date.Date && (s.EndDate == null || s.EndDate.Value.Date >= date.Date)).ToList();

                foreach (var sch in daySchedules)
                {
                    if (!courseClasses.TryGetValue(sch.CourseClassId, out var cc)) continue;
                    if (!subjects.TryGetValue(cc.SubjectId, out var sb)) continue;

                    result.Add(new WeeklyScheduleItemViewModel
                    {
                        Date = date,
                        Session = sch.Session,
                        ClassCode = cc.ClassCode,
                        SubjectName = sb.SubjectName,
                        Room = sch.Room
                    });
                }
            }

            return result.OrderBy(r => r.Date).ThenBy(r => r.Session).ToList();
        }

        private List<CourseGradeStatViewModel> BuildCourseGrades(
            List<Enrollment> enrollments,
            Dictionary<int, CourseClass> courseClasses,
            Dictionary<int, Subject> subjects,
            Dictionary<int, Grade> grades)
        {
            var result = new List<CourseGradeStatViewModel>();
            foreach (var enrollment in enrollments)
            {
                courseClasses.TryGetValue(enrollment.CourseClassId, out var cc);
                if (cc == null) continue;
                subjects.TryGetValue(cc.SubjectId, out var sb);
                grades.TryGetValue(enrollment.Id, out var grade);

                var total = grade?.TotalScore;
                var classification = GetClassification(total);

                result.Add(new CourseGradeStatViewModel
                {
                    CourseClassId = cc.Id,
                    ClassCode = cc.ClassCode,
                    SubjectName = sb?.SubjectName ?? string.Empty,
                    Credits = sb?.Credits ?? 0,
                    MidtermScore = grade?.MidtermScore,
                    FinalScore = grade?.FinalScore,
                    TotalScore = total,
                    LetterGrade = grade?.LetterGrade,
                    Classification = classification
                });
            }
            return result.OrderBy(c => c.ClassCode).ToList();
        }

        private AttendanceSummaryViewModel BuildAttendanceSummary(
            List<Enrollment> enrollments,
            Dictionary<int, CourseClass> courseClasses,
            Dictionary<int, Subject> subjects,
            List<Attendance> attendances)
        {
            var summary = new AttendanceSummaryViewModel();

            var totalPresent = 0;
            var totalSessions = 0;

            foreach (var enrollment in enrollments)
            {
                var courseAttendance = attendances
                    .Where(a => a.CourseClassId == enrollment.CourseClassId)
                    .ToList();

                var sessions = courseAttendance.Count;
                var presentSessions = courseAttendance.Count(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late);

                if (!courseClasses.TryGetValue(enrollment.CourseClassId, out var cc)) continue;
                subjects.TryGetValue(cc.SubjectId, out var sb);

                var rate = sessions > 0 ? Math.Round(presentSessions * 100.0 / sessions, 2) : 0;

                summary.Courses.Add(new AttendanceCourseDetailViewModel
                {
                    CourseClassId = enrollment.CourseClassId,
                    ClassCode = cc.ClassCode,
                    SubjectName = sb?.SubjectName ?? string.Empty,
                    PresentSessions = presentSessions,
                    TotalSessions = sessions,
                    AttendanceRate = rate
                });

                totalPresent += presentSessions;
                totalSessions += sessions;
            }

            summary.TotalPresent = totalPresent;
            summary.TotalAbsent = totalSessions - totalPresent;
            summary.AverageRate = totalSessions > 0 ? Math.Round(totalPresent * 100.0 / totalSessions, 2) : 0;

            summary.AbsentDays = attendances
                .Where(a => a.Status == AttendanceStatus.Absent || a.Status == AttendanceStatus.Excused)
                .Select(a =>
                {
                    courseClasses.TryGetValue(a.CourseClassId, out var cc);
                    subjects.TryGetValue(cc?.SubjectId ?? 0, out var sb);
                    return new AbsentDayViewModel
                    {
                        Date = a.AttendanceDate,
                        Session = a.Session,
                        ClassCode = cc?.ClassCode ?? string.Empty,
                        SubjectName = sb?.SubjectName ?? string.Empty,
                        Note = a.Note
                    };
                })
                .OrderBy(a => a.Date)
                .ThenBy(a => a.Session)
                .ToList();

            return summary;
        }

        private double? CalculateGpa(List<CourseGradeStatViewModel> courses)
        {
            double total = 0;
            int credits = 0;
            foreach (var c in courses.Where(c => c.TotalScore.HasValue))
            {
                total += c.TotalScore!.Value * c.Credits;
                credits += c.Credits;
            }
            return credits > 0 ? Math.Round(total / credits, 2) : null;
        }

        private string GetClassification(double? score)
        {
            if (!score.HasValue) return "Ch?a có";
            var s = score.Value;
            if (s >= 9.0) return "Xu?t s?c";
            if (s >= 8.0) return "Gi?i";
            if (s >= 7.0) return "Khá";
            if (s >= 5.0) return "Trung bình";
            return "Y?u";
        }
    }
}
