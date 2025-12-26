using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service quản lý Schedule
    /// </summary>
    public interface IScheduleService
    {
        List<ScheduleListViewModel> GetAll();
        List<ScheduleListViewModel> GetByCourseClassId(int courseClassId);
        TimetableViewModel GetStudentTimetable(int studentId, string semester);
        TimetableViewModel GetLecturerTimetable(int lecturerId, string semester);
        Schedule? GetById(int id);
        bool Create(ScheduleFormViewModel model);
        bool Update(ScheduleFormViewModel model);
        bool Delete(int id);
    }

    public class ScheduleService : IScheduleService
    {
        private readonly INotificationService _notificationService;

        public ScheduleService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public List<ScheduleListViewModel> GetAll()
        {
            return FakeDatabase.Schedules.Select(s =>
            {
                var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == s.CourseClassId);
                var subject = courseClass != null
                    ? FakeDatabase.Subjects.FirstOrDefault(sub => sub.Id == courseClass.SubjectId)
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
            }).OrderBy(s => s.DayOfWeek).ThenBy(s => s.TimeRange).ToList();
        }

        public List<ScheduleListViewModel> GetByCourseClassId(int courseClassId)
        {
            return FakeDatabase.Schedules
                .Where(s => s.CourseClassId == courseClassId)
                .Select(s =>
                {
                    var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == s.CourseClassId);
                    var subject = courseClass != null
                        ? FakeDatabase.Subjects.FirstOrDefault(sub => sub.Id == courseClass.SubjectId)
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
                }).OrderBy(s => s.DayOfWeek).ToList();
        }

        public TimetableViewModel GetStudentTimetable(int studentId, string semester)
        {
            var enrolledClasses = FakeDatabase.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved)
                .Select(e => e.CourseClassId)
                .ToList();

            var courseClasses = FakeDatabase.CourseClasses
                .Where(c => enrolledClasses.Contains(c.Id) && c.Semester == semester)
                .ToList();

            var semesterParts = semester.Split('-');
            var semesterNumber = semesterParts.Length > 0 ? semesterParts[0] : "HK1";
            var year = semesterParts.Length > 1 ? semesterParts[1] : DateTime.Now.Year.ToString();
            
            var semesterLabel = $"{semesterNumber} - School year {year} - {int.Parse(year) + 1}";

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
                Semester = semester,
                SemesterLabel = semesterLabel,
                WeekLabel = weekLabel,
                DayHeaders = dayHeaders,
                Schedule = new Dictionary<DayOfWeek, List<TimetableSlot>>()
            };

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                timetable.Schedule[day] = new List<TimetableSlot>();
            }

            foreach (var courseClass in courseClasses)
            {
                var schedules = FakeDatabase.Schedules.Where(s => s.CourseClassId == courseClass.Id).ToList();
                var subject = FakeDatabase.Subjects.FirstOrDefault(s => s.Id == courseClass.SubjectId);
                var lecturer = FakeDatabase.Lecturers.FirstOrDefault(l => l.Id == courseClass.LecturerId);

                foreach (var schedule in schedules)
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

        public TimetableViewModel GetLecturerTimetable(int lecturerId, string semester)
        {
            var courseClasses = FakeDatabase.CourseClasses
                .Where(c => c.LecturerId == lecturerId && c.Semester == semester)
                .ToList();

            var semesterParts = semester.Split('-');
            var semesterNumber = semesterParts.Length > 0 ? semesterParts[0] : "HK1";
            var year = semesterParts.Length > 1 ? semesterParts[1] : DateTime.Now.Year.ToString();
            
            var semesterLabel = $"{semesterNumber} - School year {year} - {int.Parse(year) + 1}";

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
                Semester = semester,
                SemesterLabel = semesterLabel,
                WeekLabel = weekLabel,
                DayHeaders = dayHeaders,
                Schedule = new Dictionary<DayOfWeek, List<TimetableSlot>>()
            };

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                timetable.Schedule[day] = new List<TimetableSlot>();
            }

            foreach (var courseClass in courseClasses)
            {
                var schedules = FakeDatabase.Schedules.Where(s => s.CourseClassId == courseClass.Id).ToList();
                var subject = FakeDatabase.Subjects.FirstOrDefault(s => s.Id == courseClass.SubjectId);

                foreach (var schedule in schedules)
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

        public Schedule? GetById(int id)
        {
            return FakeDatabase.Schedules.FirstOrDefault(s => s.Id == id);
        }

        public bool Create(ScheduleFormViewModel model)
        {
            // Check for conflicts
            if (HasConflict(model.CourseClassId, model.DayOfWeek, model.Period, model.Room, null))
            {
                return false;
            }

            var schedule = new Schedule
            {
                Id = FakeDatabase.GetNextScheduleId(),
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

            FakeDatabase.Schedules.Add(schedule);
            NotifyClassScheduleChange(model.CourseClassId, "New schedule has been added");

            return true;
        }

        public bool Update(ScheduleFormViewModel model)
        {
            var schedule = GetById(model.Id ?? 0);
            if (schedule == null) return false;

            // Check for conflicts (excluding current schedule)
            if (HasConflict(model.CourseClassId, model.DayOfWeek, model.Period, model.Room, schedule.Id))
            {
                return false;
            }

            schedule.DayOfWeek = model.DayOfWeek;
            schedule.Session = model.Session;
            schedule.Period = model.Period;
            schedule.StartTime = model.StartTime;
            schedule.EndTime = model.EndTime;
            schedule.Room = model.Room;
            schedule.EffectiveDate = model.EffectiveDate;
            schedule.EndDate = model.EndDate;

            NotifyClassScheduleChange(schedule.CourseClassId, "Schedule has been updated");

            return true;
        }

        public bool Delete(int id)
        {
            var schedule = GetById(id);
            if (schedule == null) return false;

            var courseClassId = schedule.CourseClassId;
            FakeDatabase.Schedules.Remove(schedule);

            NotifyClassScheduleChange(courseClassId, "Schedule has been removed");

            return true;
        }

        /// <summary>
        /// Check if there's a room conflict at the given day/period
        /// </summary>
        private bool HasConflict(int courseClassId, DayOfWeek day, string period, string room, int? excludeScheduleId)
        {
            return FakeDatabase.Schedules.Any(s =>
                s.DayOfWeek == day &&
                s.Period == period &&
                s.Room == room &&
                (!excludeScheduleId.HasValue || s.Id != excludeScheduleId.Value));
        }

        /// <summary>
        /// Check if lecturer has conflict at the given day/period
        /// </summary>
        private bool HasLecturerConflict(int lecturerId, DayOfWeek day, string period, int? excludeScheduleId)
        {
            if (lecturerId == 0) return false;

            var lecturerClassIds = FakeDatabase.CourseClasses
                .Where(c => c.LecturerId == lecturerId)
                .Select(c => c.Id)
                .ToList();

            return FakeDatabase.Schedules.Any(s =>
                lecturerClassIds.Contains(s.CourseClassId) &&
                s.DayOfWeek == day &&
                s.Period == period &&
                (!excludeScheduleId.HasValue || s.Id != excludeScheduleId.Value));
        }

        private void NotifyClassScheduleChange(int courseClassId, string message)
        {
            var enrollments = FakeDatabase.Enrollments
                .Where(e => e.CourseClassId == courseClassId && e.Status == EnrollmentStatus.Approved)
                .ToList();

            foreach (var enrollment in enrollments)
            {
                var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == enrollment.StudentId);
                if (student != null)
                {
                    _notificationService.CreateNotification(
                        student.UserId,
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
