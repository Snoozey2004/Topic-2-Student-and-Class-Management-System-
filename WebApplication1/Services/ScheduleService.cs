using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service qu?n lý Schedule
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
            // L?y các l?p mà sinh viên ?ã ??ng ký
            var enrolledClasses = FakeDatabase.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved)
                .Select(e => e.CourseClassId)
                .ToList();

            var courseClasses = FakeDatabase.CourseClasses
                .Where(c => enrolledClasses.Contains(c.Id) && c.Semester == semester)
                .ToList();

            var timetable = new TimetableViewModel
            {
                Semester = semester,
                Schedule = new Dictionary<DayOfWeek, List<TimetableSlot>>()
            };

            // Kh?i t?o dictionary cho t?t c? các ngày trong tu?n
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                if (day != DayOfWeek.Sunday && day != DayOfWeek.Saturday)
                {
                    timetable.Schedule[day] = new List<TimetableSlot>();
                }
            }

            foreach (var courseClass in courseClasses)
            {
                var schedules = FakeDatabase.Schedules.Where(s => s.CourseClassId == courseClass.Id).ToList();
                var subject = FakeDatabase.Subjects.FirstOrDefault(s => s.Id == courseClass.SubjectId);
                var lecturer = FakeDatabase.Lecturers.FirstOrDefault(l => l.Id == courseClass.LecturerId);

                foreach (var schedule in schedules)
                {
                    // Thêm t?t c? các l?ch, bao g?m Th? 7 và Ch? nh?t
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

            var timetable = new TimetableViewModel
            {
                Semester = semester,
                Schedule = new Dictionary<DayOfWeek, List<TimetableSlot>>()
            };

            // Kh?i t?o dictionary cho t?t c? các ngày trong tu?n
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                if (day != DayOfWeek.Sunday && day != DayOfWeek.Saturday)
                {
                    timetable.Schedule[day] = new List<TimetableSlot>();
                }
            }

            foreach (var courseClass in courseClasses)
            {
                var schedules = FakeDatabase.Schedules.Where(s => s.CourseClassId == courseClass.Id).ToList();
                var subject = FakeDatabase.Subjects.FirstOrDefault(s => s.Id == courseClass.SubjectId);

                foreach (var schedule in schedules)
                {
                    // Thêm t?t c? các l?ch, bao g?m Th? 7 và Ch? nh?t
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

        public Schedule? GetById(int id)
        {
            return FakeDatabase.Schedules.FirstOrDefault(s => s.Id == id);
        }

        public bool Create(ScheduleFormViewModel model)
        {
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

            // T?o thông báo cho sinh viên trong l?p
            NotifyClassScheduleChange(model.CourseClassId, "L?ch h?c m?i ?ã ???c thêm");

            return true;
        }

        public bool Update(ScheduleFormViewModel model)
        {
            var schedule = GetById(model.Id ?? 0);
            if (schedule == null) return false;

            schedule.DayOfWeek = model.DayOfWeek;
            schedule.Session = model.Session;
            schedule.Period = model.Period;
            schedule.StartTime = model.StartTime;
            schedule.EndTime = model.EndTime;
            schedule.Room = model.Room;
            schedule.EffectiveDate = model.EffectiveDate;
            schedule.EndDate = model.EndDate;

            // T?o thông báo cho sinh viên trong l?p
            NotifyClassScheduleChange(schedule.CourseClassId, "L?ch h?c ?ã ???c c?p nh?t");

            return true;
        }

        public bool Delete(int id)
        {
            var schedule = GetById(id);
            if (schedule == null) return false;

            FakeDatabase.Schedules.Remove(schedule);

            // T?o thông báo cho sinh viên trong l?p
            NotifyClassScheduleChange(schedule.CourseClassId, "L?ch h?c ?ã b? xóa");

            return true;
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
                        "Thay ??i l?ch h?c",
                        message,
                        NotificationType.Schedule,
                        "/Student/Schedule"
                    );
                }
            }
        }
    }
}
