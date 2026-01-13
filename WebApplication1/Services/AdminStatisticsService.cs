using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    public interface IAdminStatisticsService
    {
        AdminStatisticsViewModel GetStatistics();
    }

    public class AdminStatisticsService : IAdminStatisticsService
    {
        private readonly ApplicationDbContext _db;

        public AdminStatisticsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public AdminStatisticsViewModel GetStatistics()
        {
            var students = _db.Students.AsNoTracking().ToList();
            var lecturers = _db.Lecturers.AsNoTracking().ToList();
            var courseClasses = _db.CourseClasses.AsNoTracking().ToList();
            var subjects = _db.Subjects.AsNoTracking().ToDictionary(s => s.Id, s => s);

            var grades = _db.Grades
                .AsNoTracking()
                .Where(g => g.TotalScore.HasValue)
                .ToList();

            var attendances = _db.Attendances.AsNoTracking().ToList();
            var enrollments = _db.Enrollments.AsNoTracking().Where(e => e.Status == EnrollmentStatus.Approved).ToList();

            var model = new AdminStatisticsViewModel
            {
                TotalStudents = students.Count,
                TotalLecturers = lecturers.Count,
                TotalClasses = courseClasses.Count,
            };

            // Score stats
            if (grades.Count > 0)
            {
                model.AverageScore = Math.Round(grades.Average(g => g.TotalScore!.Value), 2);
                model.PassRate = Math.Round(grades.Count(g => g.IsPassed) * 100.0 / grades.Count, 2);

                model.ScoreDistribution = new List<ScoreDistributionItem>
                {
                    new ScoreDistributionItem { Label = "9-10", Count = grades.Count(g => g.TotalScore >= 9) },
                    new ScoreDistributionItem { Label = "8-8.9", Count = grades.Count(g => g.TotalScore >= 8 && g.TotalScore < 9) },
                    new ScoreDistributionItem { Label = "7-7.9", Count = grades.Count(g => g.TotalScore >= 7 && g.TotalScore < 8) },
                    new ScoreDistributionItem { Label = "5-6.9", Count = grades.Count(g => g.TotalScore >= 5 && g.TotalScore < 7) },
                    new ScoreDistributionItem { Label = "<5", Count = grades.Count(g => g.TotalScore < 5) }
                };

                // top students by GPA (using grade * credits)
                var enrollIds = grades.Select(g => g.EnrollmentId).Distinct().ToList();
                var enrolls = enrollments.Where(e => enrollIds.Contains(e.Id)).ToList();
                var classIds = enrolls.Select(e => e.CourseClassId).Distinct().ToList();
                var classCredits = _db.CourseClasses
                    .AsNoTracking()
                    .Where(c => classIds.Contains(c.Id))
                    .ToDictionary(c => c.Id, c => subjects.TryGetValue(c.SubjectId, out var sb) ? sb.Credits : 0);

                var gpaByStudent = grades
                    .GroupBy(g => g.StudentId)
                    .Select(g => new
                    {
                        StudentId = g.Key,
                        TotalPoints = g.Sum(x =>
                        {
                            var enrollment = enrolls.FirstOrDefault(e => e.Id == x.EnrollmentId);
                            if (enrollment == null) return 0;
                            if (!classCredits.TryGetValue(enrollment.CourseClassId, out var cr)) cr = 0;
                            return x.TotalScore!.Value * cr;
                        }),
                        TotalCredits = g.Sum(x =>
                        {
                            var enrollment = enrolls.FirstOrDefault(e => e.Id == x.EnrollmentId);
                            if (enrollment == null) return 0;
                            return classCredits.TryGetValue(enrollment.CourseClassId, out var cr) ? cr : 0;
                        })
                    })
                    .Select(x => new
                    {
                        x.StudentId,
                        GPA = x.TotalCredits > 0 ? Math.Round(x.TotalPoints / x.TotalCredits, 2) : 0
                    })
                    .OrderByDescending(x => x.GPA)
                    .Take(10)
                    .ToList();

                var studentDict = students.ToDictionary(s => s.Id, s => s);
                model.TopStudents = gpaByStudent
                    .Where(x => x.GPA > 0 && studentDict.ContainsKey(x.StudentId))
                    .Select(x => new TopStudentItem
                    {
                        StudentId = x.StudentId,
                        StudentCode = studentDict[x.StudentId].StudentCode,
                        StudentName = studentDict[x.StudentId].FullName,
                        GPA = x.GPA
                    })
                    .ToList();

                model.WarningLowScoreCount = grades
                    .Where(g => g.TotalScore < 4.0)
                    .Select(g => g.StudentId)
                    .Distinct()
                    .Count();

                // Trend by semester
                var classById = courseClasses.ToDictionary(c => c.Id, c => c);
                model.ScoreTrendBySemester = grades
                    .GroupBy(g =>
                    {
                        if (!classById.TryGetValue(g.CourseClassId, out var cc)) return "Unknown";
                        return cc.Semester;
                    })
                    .Select(gr => new SemesterTrendItem
                    {
                        Semester = gr.Key,
                        Value = Math.Round(gr.Average(x => x.TotalScore!.Value), 2)
                    })
                    .OrderBy(x => x.Semester)
                    .ToList();
            }

            // Attendance stats
            if (attendances.Count > 0)
            {
                var grouped = attendances.GroupBy(a => new { a.CourseClassId, a.AttendanceDate.Date, a.Session });
                var totalSessions = grouped.Count();
                var classById = courseClasses.ToDictionary(c => c.Id, c => c);

                // Each grouping has many records (per student). Attendance rate per class overall
                var attendanceRatesByClass = attendances
                    .GroupBy(a => a.CourseClassId)
                    .Select(g =>
                    {
                        var total = g.Count();
                        var present = g.Count(x => x.Status == AttendanceStatus.Present || x.Status == AttendanceStatus.Late);
                        var rate = total > 0 ? Math.Round(present * 100.0 / total, 2) : 0;
                        classById.TryGetValue(g.Key, out var cc);
                        subjects.TryGetValue(cc?.SubjectId ?? 0, out var sb);
                        var studentCount = enrollments.Count(e => e.CourseClassId == g.Key);
                        return new AttendanceDistributionItem
                        {
                            ClassCode = cc?.ClassCode ?? string.Empty,
                            SubjectName = sb?.SubjectName ?? string.Empty,
                            AttendanceRate = rate,
                            StudentCount = studentCount
                        };
                    })
                    .OrderByDescending(a => a.AttendanceRate)
                    .ToList();

                model.AttendanceByClass = attendanceRatesByClass;

                var presentTotal = attendances.Count(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late);
                model.AverageAttendanceRate = attendances.Count > 0 ? Math.Round(presentTotal * 100.0 / attendances.Count, 2) : 0;

                model.HighAbsenceStudentCount = attendances
                    .GroupBy(a => a.StudentId)
                    .Count(g =>
                    {
                        var total = g.Count();
                        var present = g.Count(x => x.Status == AttendanceStatus.Present || x.Status == AttendanceStatus.Late);
                        var rate = total > 0 ? present * 100.0 / total : 0;
                        return rate < 80;
                    });

                model.AttendanceTrendBySemester = attendances
                    .GroupBy(a =>
                    {
                        if (!classById.TryGetValue(a.CourseClassId, out var cc)) return "Unknown";
                        return cc.Semester;
                    })
                    .Select(gr =>
                    {
                        var total = gr.Count();
                        var present = gr.Count(x => x.Status == AttendanceStatus.Present || x.Status == AttendanceStatus.Late);
                        var rate = total > 0 ? Math.Round(present * 100.0 / total, 2) : 0;
                        return new SemesterTrendItem { Semester = gr.Key, Value = rate };
                    })
                    .OrderBy(x => x.Semester)
                    .ToList();
            }

            return model;
        }
    }
}
