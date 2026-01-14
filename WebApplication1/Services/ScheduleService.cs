using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service quản lý Schedule (SQL Server + EF Core)
    /// </summary>
    public interface IScheduleService
    {
        List<ScheduleListViewModel> GetAll();
        List<ScheduleListViewModel> GetByCourseClassId(int courseClassId);
        TimetableViewModel GetStudentTimetable(int studentId, string? semester);
        TimetableViewModel GetLecturerTimetable(int lecturerId, string? semester);
        List<string> GetStudentSemesters(int studentId);
        List<string> GetLecturerSemesters(int lecturerId);
        Schedule? GetById(int id);
        bool Create(ScheduleFormViewModel model);
        bool Update(ScheduleFormViewModel model);
        bool Delete(int id);
    }

    public class ScheduleService : IScheduleService
    {
        private readonly ApplicationDbContext _db;
        private readonly INotificationService _notificationService;
        private static readonly Dictionary<string, (string Start, string End)> PeriodTimes = new()
        {
            { "Period 1-3", ("07:00", "09:30") },
            { "Period 4-6", ("09:45", "12:15") },
            { "Period 7-9", ("13:00", "15:30") },
            { "Period 10-12", ("15:45", "18:15") },
            { "Period 13-15", ("18:30", "21:00") }
        };

        public ScheduleService(ApplicationDbContext db, INotificationService notificationService)
        {
            _db = db;
            _notificationService = notificationService;
        }

        private void ApplyDefaultTimesIfMissing(ScheduleFormViewModel model)
        {
            if (!PeriodTimes.TryGetValue(model.Period ?? string.Empty, out var times)) return;
            if (string.IsNullOrWhiteSpace(model.StartTime)) model.StartTime = times.Start;
            if (string.IsNullOrWhiteSpace(model.EndTime)) model.EndTime = times.End;
        }

        public List<ScheduleListViewModel> GetAll()
        {
            var schedules = _db.Schedules.ToList();   // ✅ tách khỏi EF

            return schedules
                .Select(s =>
                {
                    var courseClass = _db.CourseClasses.FirstOrDefault(c => c.Id == s.CourseClassId);
                    var subject = courseClass != null
                        ? _db.Subjects.FirstOrDefault(sub => sub.Id == courseClass.SubjectId)
                        : null;

                    return new ScheduleListViewModel
                    {
                        Id = s.Id,
                        ClassCode = courseClass?.ClassCode ?? "",
                        SubjectName = subject?.SubjectName ?? "",
                        DayOfWeek = s.DayOfWeek.ToString(),
                        Session = s.Session,
                        Period = s.Period,
                        TimeRange = $"{s.StartTime} - {s.EndTime}",
                        Room = s.Room,
                        EffectiveDate = s.EffectiveDate
                    };
                })
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.TimeRange)
                .ToList();
        }


        public List<ScheduleListViewModel> GetByCourseClassId(int courseClassId)
        {
            // 1) Lấy schedule ra khỏi EF trước
            var schedules = _db.Schedules
                .Where(s => s.CourseClassId == courseClassId)
                .ToList();

            // 2) Map sang ViewModel bằng C# (không còn expression tree)
            return schedules
                .Select(s =>
                {
                    var courseClass = _db.CourseClasses.FirstOrDefault(c => c.Id == s.CourseClassId);

                    var subject = courseClass != null
                        ? _db.Subjects.FirstOrDefault(sub => sub.Id == courseClass.SubjectId)   
                        : null;

                    return new ScheduleListViewModel
                    {
                        Id = s.Id,
                        ClassCode = courseClass?.ClassCode ?? "",
                        SubjectName = subject?.SubjectName ?? "",
                        DayOfWeek = s.DayOfWeek.ToString(),
                        Session = s.Session,
                        Period = s.Period,
                        TimeRange = $"{s.StartTime} - {s.EndTime}",
                        Room = s.Room,
                        EffectiveDate = s.EffectiveDate
                    };
                })
                .OrderBy(x => x.DayOfWeek)
                .ToList();
        }



        public TimetableViewModel GetStudentTimetable(int studentId, string? semester)
        {
            var enrollments = _db.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved)
                .ToList();

            var enrolledClassIds = enrollments.Select(e => e.CourseClassId).Distinct().ToList();

            string? selectedSemester = semester;
            if (string.IsNullOrWhiteSpace(selectedSemester))
            {
                var semesters = _db.CourseClasses
                    .AsNoTracking()
                    .Where(c => enrolledClassIds.Contains(c.Id))
                    .Select(c => c.Semester)
                    .Distinct()
                    .ToList();

                selectedSemester = GetLatestSemesterOrDefault(semesters, null);
            }

            var courseClassesQuery = _db.CourseClasses
                .AsNoTracking()
                .Where(c => enrolledClassIds.Contains(c.Id));

            if (!string.IsNullOrWhiteSpace(selectedSemester))
            {
                courseClassesQuery = courseClassesQuery.Where(c => c.Semester == selectedSemester);
            }

            var courseClasses = courseClassesQuery.ToList();

            var semesterLabel = !string.IsNullOrWhiteSpace(selectedSemester)
                ? BuildSemesterLabel(selectedSemester)
                : "All semesters";

            var currentDate = DateTime.Now;
            var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);
            var weekNumber = GetWeekOfYear(currentDate);
            var weekLabel = $"Week {weekNumber} [from date {startOfWeek:dd/MM/yyyy} to date {startOfWeek.AddDays(6):dd/MM/yyyy}]";

            var dayHeaders = new List<DayHeaderViewModel>();
            for (int i = 0; i < 7; i++)
            {
                var date = startOfWeek.AddDays(i);
                dayHeaders.Add(new DayHeaderViewModel
                {
                    DayName = date.DayOfWeek.ToString(),
                    DateDisplay = $"({date:dd/MM})",
                    DayOfWeek = date.DayOfWeek
                });
            }

            var timetable = new TimetableViewModel
            {
                Semester = selectedSemester ?? "All",
                SemesterLabel = semesterLabel,
                WeekLabel = weekLabel,
                DayHeaders = dayHeaders,
                Schedule = new Dictionary<DayOfWeek, List<TimetableSlot>>()
            };

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                timetable.Schedule[day] = new List<TimetableSlot>();

            // preload subjects + lecturers để đỡ query lặp
            var subjectIds = courseClasses.Select(c => c.SubjectId).Distinct().ToList();
            var lecturerIds = courseClasses.Select(c => c.LecturerId).Distinct().ToList();

            var subjects = _db.Subjects.AsNoTracking()
                .Where(s => subjectIds.Contains(s.Id))
                .ToDictionary(s => s.Id, s => s);

            var lecturers = _db.Lecturers.AsNoTracking()
                .Where(l => lecturerIds.Contains(l.Id))
                .ToDictionary(l => l.Id, l => l);

            var classIds = courseClasses.Select(c => c.Id).ToList();
            var schedules = _db.Schedules.AsNoTracking()
                .Where(s => classIds.Contains(s.CourseClassId))
                .ToList();

            foreach (var courseClass in courseClasses)
            {
                subjects.TryGetValue(courseClass.SubjectId, out var subject);
                lecturers.TryGetValue(courseClass.LecturerId, out var lecturer);

                foreach (var schedule in schedules.Where(s => s.CourseClassId == courseClass.Id))
                {
                    timetable.Schedule[schedule.DayOfWeek].Add(new TimetableSlot
                    {
                        ClassCode = courseClass.ClassCode,
                        SubjectName = subject?.SubjectName ?? "",
                        LecturerName = lecturer?.FullName ?? "",
                        Room = schedule.Room,
                        Period = schedule.Period,
                        TimeRange = $"{schedule.StartTime} - {schedule.EndTime}"
                    });
                }
            }

            return timetable;
        }

        public TimetableViewModel GetLecturerTimetable(int lecturerId, string? semester)
        {
            var classQuery = _db.CourseClasses
                .AsNoTracking()
                .Where(c => c.LecturerId == lecturerId);

            if (string.IsNullOrWhiteSpace(semester))
            {
                var semesters = classQuery
                    .Select(c => c.Semester)
                    .Distinct()
                    .ToList();

                semester = GetLatestSemesterOrDefault(semesters, null);
            }

            if (!string.IsNullOrWhiteSpace(semester))
            {
                classQuery = classQuery.Where(c => c.Semester == semester);
            }

            var courseClasses = classQuery.ToList();

            var semesterLabel = !string.IsNullOrWhiteSpace(semester) ? BuildSemesterLabel(semester) : "All semesters";

            var currentDate = DateTime.Now;
            var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);
            var weekNumber = GetWeekOfYear(currentDate);
            var weekLabel = $"Week {weekNumber} [from date {startOfWeek:dd/MM/yyyy} to date {startOfWeek.AddDays(6):dd/MM/yyyy}]";

            var dayHeaders = new List<DayHeaderViewModel>();
            for (int i = 0; i < 7; i++)
            {
                var date = startOfWeek.AddDays(i);
                dayHeaders.Add(new DayHeaderViewModel
                {
                    DayName = date.DayOfWeek.ToString(),
                    DateDisplay = $"({date:dd/MM})",
                    DayOfWeek = date.DayOfWeek
                });
            }

            var timetable = new TimetableViewModel
            {
                Semester = semester ?? "All",
                SemesterLabel = semesterLabel,
                WeekLabel = weekLabel,
                DayHeaders = dayHeaders,
                Schedule = new Dictionary<DayOfWeek, List<TimetableSlot>>()
            };

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                timetable.Schedule[day] = new List<TimetableSlot>();

            var subjectIds = courseClasses.Select(c => c.SubjectId).Distinct().ToList();
            var subjects = _db.Subjects.AsNoTracking()
                .Where(s => subjectIds.Contains(s.Id))
                .ToDictionary(s => s.Id, s => s);

            var classIds = courseClasses.Select(c => c.Id).ToList();
            var schedules = _db.Schedules.AsNoTracking()
                .Where(s => classIds.Contains(s.CourseClassId))
                .ToList();

            foreach (var courseClass in courseClasses)
            {
                subjects.TryGetValue(courseClass.SubjectId, out var subject);

                foreach (var schedule in schedules.Where(s => s.CourseClassId == courseClass.Id))
                {
                    timetable.Schedule[schedule.DayOfWeek].Add(new TimetableSlot
                    {
                        ClassCode = courseClass.ClassCode,
                        SubjectName = subject?.SubjectName ?? "",
                        LecturerName = "",
                        Room = schedule.Room,
                        Period = schedule.Period,
                        TimeRange = $"{schedule.StartTime} - {schedule.EndTime}"
                    });
                }
            }

            return timetable;
        }

        private int GetWeekOfYear(DateTime date)
        {
            var jan1 = new DateTime(date.Year, 1, 1);
            var daysOffset = (int)jan1.DayOfWeek - (int)DayOfWeek.Monday;
            if (daysOffset < 0) daysOffset += 7;
            var firstMonday = jan1.AddDays(-daysOffset);
            var weekNumber = ((date - firstMonday).Days / 7) + 1;
            return weekNumber;
        }

        private string? GetLatestSemesterOrDefault(IEnumerable<string> semesters, string? defaultSemester)
        {
            var parsed = semesters
                .Select(s => (raw: s, parsed: ParseSemester(s)))
                .Where(x => x.parsed.HasValue)
                .OrderByDescending(x => x.parsed.Value.year)
                .ThenByDescending(x => x.parsed.Value.term)
                .FirstOrDefault();

            if (parsed.parsed.HasValue) return parsed.raw;
            return semesters.FirstOrDefault() ?? defaultSemester;
        }

        private string BuildSemesterLabel(string semester)
        {
            var parts = semester.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var semesterNumber = parts.Length > 0 ? parts[0] : "HK1";
            var year = parts.Length > 1 ? parts[1] : DateTime.Now.Year.ToString();
            return $"{semesterNumber} - School year {year} - {int.Parse(year) + 1}";
        }

        public List<string> GetStudentSemesters(int studentId)
        {
            var enrollments = _db.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved)
                .Select(e => e.CourseClassId)
                .Distinct()
                .ToList();

            var semesters = _db.CourseClasses
                .AsNoTracking()
                .Where(c => enrollments.Contains(c.Id))
                .Select(c => c.Semester)
                .Distinct()
                .ToList();

            return semesters
                .Select(s => (raw: s, parsed: ParseSemester(s)))
                .Where(x => x.parsed.HasValue)
                .OrderByDescending(x => x.parsed.Value.year)
                .ThenByDescending(x => x.parsed.Value.term)
                .Select(x => x.raw)
                .ToList();
        }

        public List<string> GetLecturerSemesters(int lecturerId)
        {
            var semesterList = _db.CourseClasses
                .AsNoTracking()
                .Where(c => c.LecturerId == lecturerId)
                .Select(c => c.Semester)
                .Distinct()
                .ToList();

            return semesterList
                .Select(s => (raw: s, parsed: ParseSemester(s)))
                .Where(x => x.parsed.HasValue)
                .OrderByDescending(x => x.parsed.Value.year)
                .ThenByDescending(x => x.parsed.Value.term)
                .Select(x => x.raw)
                .ToList();
        }

        private (int year, int term)? ParseSemester(string semester)
        {
            if (string.IsNullOrWhiteSpace(semester)) return null;
            var parts = semester.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length < 2) return null;
            var termPart = parts[0].Replace("HK", "", StringComparison.OrdinalIgnoreCase);
            if (!int.TryParse(termPart, out var term)) return null;
            if (!int.TryParse(parts[1], out var year)) return null;
            return (year, term);
        }

        public Schedule? GetById(int id)
        {
            return _db.Schedules.FirstOrDefault(s => s.Id == id);
        }

        public bool Create(ScheduleFormViewModel model)
        {
            ApplyDefaultTimesIfMissing(model);
            // Room conflict
            if (HasConflict(model.CourseClassId, model.DayOfWeek, model.Period, model.Room, null))
                return false;

            // Lecturer conflict (optional nhưng nên check)
            var lecturerId = _db.CourseClasses
                .AsNoTracking()
                .Where(c => c.Id == model.CourseClassId)
                .Select(c => c.LecturerId)
                .FirstOrDefault();

            if (HasLecturerConflict(lecturerId, model.DayOfWeek, model.Period, null))
                return false;

            var schedule = new Schedule
            {
                // KHÔNG set Id nếu Id là Identity
                CourseClassId = model.CourseClassId,
                DayOfWeek = model.DayOfWeek,
                Session = model.Session,
                Period = model.Period,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Room = model.Room,
                EffectiveDate = model.EffectiveDate,
                EndDate = model.EndDate,
                CreatedDate = DateTime.Now
            };

            _db.Schedules.Add(schedule);
            _db.SaveChanges();

            NotifyClassScheduleChange(model.CourseClassId, "New schedule has been added");
            return true;
        }

        public bool Update(ScheduleFormViewModel model)
        {
            var id = model.Id ?? 0;
            var schedule = _db.Schedules.FirstOrDefault(s => s.Id == id);
            if (schedule == null) return false;

            ApplyDefaultTimesIfMissing(model);

            // Room conflict (exclude current)
            if (HasConflict(model.CourseClassId, model.DayOfWeek, model.Period, model.Room, schedule.Id))
                return false;

            // Lecturer conflict (exclude current)
            var lecturerId = _db.CourseClasses
                .AsNoTracking()
                .Where(c => c.Id == model.CourseClassId)
                .Select(c => c.LecturerId)
                .FirstOrDefault();

            if (HasLecturerConflict(lecturerId, model.DayOfWeek, model.Period, schedule.Id))
                return false;

            schedule.DayOfWeek = model.DayOfWeek;
            schedule.Session = model.Session;
            schedule.Period = model.Period;
            schedule.StartTime = model.StartTime;
            schedule.EndTime = model.EndTime;
            schedule.Room = model.Room;
            schedule.EffectiveDate = model.EffectiveDate;
            schedule.EndDate = model.EndDate;

            _db.SaveChanges();

            NotifyClassScheduleChange(schedule.CourseClassId, "Schedule has been updated");
            return true;
        }

        public bool Delete(int id)
        {
            var schedule = _db.Schedules.FirstOrDefault(s => s.Id == id);
            if (schedule == null) return false;

            var courseClassId = schedule.CourseClassId;

            _db.Schedules.Remove(schedule);
            _db.SaveChanges();

            NotifyClassScheduleChange(courseClassId, "Schedule has been removed");
            return true;
        }

        /// <summary>
        /// Check if there's a room conflict at the given day/period
        /// </summary>
        private bool HasConflict(int courseClassId, DayOfWeek day, string period, string room, int? excludeScheduleId)
        {
            var q = _db.Schedules.Where(s =>
                s.DayOfWeek == day &&
                s.Period == period &&
                s.Room == room);

            if (excludeScheduleId.HasValue)
                q = q.Where(s => s.Id != excludeScheduleId.Value);

            return q.Any();
        }

        /// <summary>
        /// Check if lecturer has conflict at the given day/period
        /// </summary>
        private bool HasLecturerConflict(int lecturerId, DayOfWeek day, string period, int? excludeScheduleId)
        {
            if (lecturerId == 0) return false;

            var lecturerClassIds = _db.CourseClasses
                .AsNoTracking()
                .Where(c => c.LecturerId == lecturerId)
                .Select(c => c.Id);

            var q = _db.Schedules.Where(s =>
                lecturerClassIds.Contains(s.CourseClassId) &&
                s.DayOfWeek == day &&
                s.Period == period);

            if (excludeScheduleId.HasValue)
                q = q.Where(s => s.Id != excludeScheduleId.Value);

            return q.Any();
        }

        private void NotifyClassScheduleChange(int courseClassId, string message)
        {
            var enrollments = _db.Enrollments
                .AsNoTracking()
                .Where(e => e.CourseClassId == courseClassId && e.Status == EnrollmentStatus.Approved)
                .ToList();

            foreach (var enrollment in enrollments)
            {
                var studentUserId = _db.Students
                    .AsNoTracking()
                    .Where(s => s.Id == enrollment.StudentId)
                    .Select(s => s.UserId)
                    .FirstOrDefault();

                if (studentUserId != 0)
                {
                    _notificationService.CreateNotification(
                        studentUserId,
                        "Schedule Update",
                        message,
                        NotificationType.Schedule,
                        "/Student/Schedule"
                    );
                }
            }
        }
    }
}
