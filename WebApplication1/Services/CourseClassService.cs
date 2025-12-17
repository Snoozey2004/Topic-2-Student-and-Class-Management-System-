using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service qu?n lý CourseClass
    /// </summary>
    public interface ICourseClassService
    {
        List<CourseClassListViewModel> GetAll(string? semester = null);
        CourseClassDetailViewModel? GetDetailById(int id);
        CourseClass? GetById(int id);
        List<CourseClassListViewModel> GetByLecturerId(int lecturerId);
        List<CourseClassListViewModel> GetByStudentId(int studentId);
        bool Create(CourseClassFormViewModel model);
        bool Update(CourseClassFormViewModel model);
        bool Delete(int id);
    }

    public class CourseClassService : ICourseClassService
    {
        public List<CourseClassListViewModel> GetAll(string? semester = null)
        {
            var query = FakeDatabase.CourseClasses.AsEnumerable();

            if (!string.IsNullOrEmpty(semester))
            {
                query = query.Where(c => c.Semester == semester);
            }

            return query.Select(c =>
            {
                var subject = FakeDatabase.Subjects.FirstOrDefault(s => s.Id == c.SubjectId);
                var lecturer = FakeDatabase.Lecturers.FirstOrDefault(l => l.Id == c.LecturerId);

                return new CourseClassListViewModel
                {
                    Id = c.Id,
                    ClassCode = c.ClassCode,
                    SubjectName = subject?.SubjectName ?? "",
                    SubjectCode = subject?.SubjectCode ?? "",
                    LecturerName = lecturer?.FullName ?? "",
                    Semester = c.Semester,
                    CurrentStudents = c.CurrentStudents,
                    MaxStudents = c.MaxStudents,
                    Room = c.Room,
                    Status = c.Status.ToString()
                };
            }).OrderBy(c => c.Semester).ThenBy(c => c.ClassCode).ToList();
        }

        public CourseClassDetailViewModel? GetDetailById(int id)
        {
            var courseClass = GetById(id);
            if (courseClass == null) return null;

            var subject = FakeDatabase.Subjects.FirstOrDefault(s => s.Id == courseClass.SubjectId);
            var lecturer = FakeDatabase.Lecturers.FirstOrDefault(l => l.Id == courseClass.LecturerId);

            var schedules = FakeDatabase.Schedules
                .Where(s => s.CourseClassId == id)
                .Select(s => new ScheduleInfoViewModel
                {
                    Id = s.Id,
                    DayOfWeek = s.DayOfWeek.ToString(),
                    Session = s.Session,
                    Period = s.Period,
                    TimeRange = $"{s.StartTime} - {s.EndTime}",
                    Room = s.Room
                }).ToList();

            var students = FakeDatabase.Enrollments
                .Where(e => e.CourseClassId == id && e.Status == EnrollmentStatus.Approved)
                .Select(e =>
                {
                    var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == e.StudentId);
                    var grade = FakeDatabase.Grades.FirstOrDefault(g => g.EnrollmentId == e.Id);

                    return new StudentInClassViewModel
                    {
                        StudentId = student?.Id ?? 0,
                        StudentCode = student?.StudentCode ?? "",
                        FullName = student?.FullName ?? "",
                        Email = student?.Email ?? "",
                        EnrollmentStatus = e.Status.ToString(),
                        TotalScore = grade?.TotalScore,
                        LetterGrade = grade?.LetterGrade
                    };
                }).ToList();

            return new CourseClassDetailViewModel
            {
                Id = courseClass.Id,
                ClassCode = courseClass.ClassCode,
                SubjectName = subject?.SubjectName ?? "",
                SubjectCode = subject?.SubjectCode ?? "",
                Credits = subject?.Credits ?? 0,
                LecturerName = lecturer?.FullName ?? "",
                Semester = courseClass.Semester,
                CurrentStudents = courseClass.CurrentStudents,
                MaxStudents = courseClass.MaxStudents,
                Room = courseClass.Room,
                Status = courseClass.Status.ToString(),
                Schedules = schedules,
                Students = students
            };
        }

        public CourseClass? GetById(int id)
        {
            return FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == id);
        }

        public List<CourseClassListViewModel> GetByLecturerId(int lecturerId)
        {
            return FakeDatabase.CourseClasses
                .Where(c => c.LecturerId == lecturerId)
                .Select(c =>
                {
                    var subject = FakeDatabase.Subjects.FirstOrDefault(s => s.Id == c.SubjectId);
                    var lecturer = FakeDatabase.Lecturers.FirstOrDefault(l => l.Id == c.LecturerId);

                    return new CourseClassListViewModel
                    {
                        Id = c.Id,
                        ClassCode = c.ClassCode,
                        SubjectName = subject?.SubjectName ?? "",
                        SubjectCode = subject?.SubjectCode ?? "",
                        LecturerName = lecturer?.FullName ?? "",
                        Semester = c.Semester,
                        CurrentStudents = c.CurrentStudents,
                        MaxStudents = c.MaxStudents,
                        Room = c.Room,
                        Status = c.Status.ToString()
                    };
                }).OrderByDescending(c => c.Semester).ToList();
        }

        public List<CourseClassListViewModel> GetByStudentId(int studentId)
        {
            var enrolledClassIds = FakeDatabase.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved)
                .Select(e => e.CourseClassId)
                .ToList();

            return FakeDatabase.CourseClasses
                .Where(c => enrolledClassIds.Contains(c.Id))
                .Select(c =>
                {
                    var subject = FakeDatabase.Subjects.FirstOrDefault(s => s.Id == c.SubjectId);
                    var lecturer = FakeDatabase.Lecturers.FirstOrDefault(l => l.Id == c.LecturerId);

                    return new CourseClassListViewModel
                    {
                        Id = c.Id,
                        ClassCode = c.ClassCode,
                        SubjectName = subject?.SubjectName ?? "",
                        SubjectCode = subject?.SubjectCode ?? "",
                        LecturerName = lecturer?.FullName ?? "",
                        Semester = c.Semester,
                        CurrentStudents = c.CurrentStudents,
                        MaxStudents = c.MaxStudents,
                        Room = c.Room,
                        Status = c.Status.ToString()
                    };
                }).OrderByDescending(c => c.Semester).ToList();
        }

        public bool Create(CourseClassFormViewModel model)
        {
            // Ki?m tra mã l?p ?ã t?n t?i
            if (FakeDatabase.CourseClasses.Any(c => c.ClassCode.Equals(model.ClassCode, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            var courseClass = new CourseClass
            {
                Id = FakeDatabase.GetNextCourseClassId(),
                ClassCode = model.ClassCode,
                SubjectId = model.SubjectId,
                LecturerId = model.LecturerId,
                Semester = model.Semester,
                MaxStudents = model.MaxStudents,
                CurrentStudents = 0,
                Room = model.Room,
                Status = model.Status,
                CreatedDate = DateTime.Now
            };

            FakeDatabase.CourseClasses.Add(courseClass);
            return true;
        }

        public bool Update(CourseClassFormViewModel model)
        {
            var courseClass = GetById(model.Id ?? 0);
            if (courseClass == null) return false;

            // Ki?m tra mã l?p trùng
            if (FakeDatabase.CourseClasses.Any(c => c.Id != courseClass.Id &&
                c.ClassCode.Equals(model.ClassCode, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            courseClass.ClassCode = model.ClassCode;
            courseClass.SubjectId = model.SubjectId;
            courseClass.LecturerId = model.LecturerId;
            courseClass.Semester = model.Semester;
            courseClass.MaxStudents = model.MaxStudents;
            courseClass.Room = model.Room;
            courseClass.Status = model.Status;

            return true;
        }

        public bool Delete(int id)
        {
            var courseClass = GetById(id);
            if (courseClass == null) return false;

            // Ki?m tra xem l?p có sinh viên ??ng ký không
            if (FakeDatabase.Enrollments.Any(e => e.CourseClassId == id))
            {
                return false;
            }

            FakeDatabase.CourseClasses.Remove(courseClass);
            return true;
        }
    }
}
