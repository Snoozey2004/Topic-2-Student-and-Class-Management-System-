using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service quản lý CourseClass (SQL Server + EF Core)
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
        private readonly ApplicationDbContext _db;

        public CourseClassService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<CourseClassListViewModel> GetAll(string? semester = null)
        {
            var query =
                from c in _db.CourseClasses
                join s in _db.Subjects on c.SubjectId equals s.Id into sj
                from subject in sj.DefaultIfEmpty()
                join l in _db.Lecturers on c.LecturerId equals l.Id into lj
                from lecturer in lj.DefaultIfEmpty()
                select new CourseClassListViewModel
                {
                    Id = c.Id,
                    ClassCode = c.ClassCode,
                    SubjectName = subject != null ? subject.SubjectName : "",
                    SubjectCode = subject != null ? subject.SubjectCode : "",
                    LecturerName = lecturer != null ? lecturer.FullName : "",
                    Semester = c.Semester,
                    CurrentStudents = c.CurrentStudents,
                    MaxStudents = c.MaxStudents,
                    Room = c.Room,
                    Status = c.Status.ToString()
                };

            if (!string.IsNullOrWhiteSpace(semester))
            {
                var sem = semester.Trim();
                query = query.Where(x => x.Semester == sem);
            }

            return query
                .AsNoTracking()
                .OrderBy(x => x.Semester)
                .ThenBy(x => x.ClassCode)
                .ToList();
        }

        public CourseClass? GetById(int id)
        {
            return _db.CourseClasses.FirstOrDefault(c => c.Id == id);
        }

        public CourseClassDetailViewModel? GetDetailById(int id)
        {
            // Load lớp (kèm Subject, Lecturer)
            var c = _db.CourseClasses
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);

            if (c == null) return null;

            var subject = _db.Subjects.AsNoTracking().FirstOrDefault(s => s.Id == c.SubjectId);
            var lecturer = _db.Lecturers.AsNoTracking().FirstOrDefault(l => l.Id == c.LecturerId);

            var schedules = _db.Schedules
                .AsNoTracking()
                .Where(s => s.CourseClassId == id)
                .Select(s => new ScheduleInfoViewModel
                {
                    Id = s.Id,
                    DayOfWeek = s.DayOfWeek.ToString(),
                    Session = s.Session,
                    Period = s.Period,
                    TimeRange = $"{s.StartTime} - {s.EndTime}",
                    Room = s.Room
                })
                .ToList();

            // Students trong lớp + grade
            var students =
                (from e in _db.Enrollments.AsNoTracking()
                 join st in _db.Students.AsNoTracking() on e.StudentId equals st.Id into stj
                 from student in stj.DefaultIfEmpty()
                 join g in _db.Grades.AsNoTracking() on e.Id equals g.EnrollmentId into gj
                 from grade in gj.DefaultIfEmpty()
                 where e.CourseClassId == id && e.Status == EnrollmentStatus.Approved
                 select new StudentInClassViewModel
                 {
                     StudentId = student != null ? student.Id : 0,
                     StudentCode = student != null ? student.StudentCode : "",
                     FullName = student != null ? student.FullName : "",
                     Email = student != null ? student.Email : "",
                     EnrollmentStatus = e.Status.ToString(),
                     TotalScore = grade != null ? grade.TotalScore : null,
                     LetterGrade = grade != null ? grade.LetterGrade : null
                 })
                .ToList();

            return new CourseClassDetailViewModel
            {
                Id = c.Id,
                ClassCode = c.ClassCode,
                SubjectName = subject?.SubjectName ?? "",
                SubjectCode = subject?.SubjectCode ?? "",
                Credits = subject?.Credits ?? 0,
                LecturerName = lecturer?.FullName ?? "",
                Semester = c.Semester,
                CurrentStudents = c.CurrentStudents,
                MaxStudents = c.MaxStudents,
                Room = c.Room,
                Status = c.Status.ToString(),
                Schedules = schedules,
                Students = students
            };
        }

        public List<CourseClassListViewModel> GetByLecturerId(int lecturerId)
        {
            var query =
                from c in _db.CourseClasses
                join s in _db.Subjects on c.SubjectId equals s.Id into sj
                from subject in sj.DefaultIfEmpty()
                join l in _db.Lecturers on c.LecturerId equals l.Id into lj
                from lecturer in lj.DefaultIfEmpty()
                where c.LecturerId == lecturerId
                select new CourseClassListViewModel
                {
                    Id = c.Id,
                    ClassCode = c.ClassCode,
                    SubjectName = subject != null ? subject.SubjectName : "",
                    SubjectCode = subject != null ? subject.SubjectCode : "",
                    LecturerName = lecturer != null ? lecturer.FullName : "",
                    Semester = c.Semester,
                    CurrentStudents = c.CurrentStudents,
                    MaxStudents = c.MaxStudents,
                    Room = c.Room,
                    Status = c.Status.ToString()
                };

            return query
                .AsNoTracking()
                .OrderByDescending(x => x.Semester)
                .ThenBy(x => x.ClassCode)
                .ToList();
        }

        public List<CourseClassListViewModel> GetByStudentId(int studentId)
        {
            // Các lớp mà student đã Approved
            var enrolledClassIds = _db.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved)
                .Select(e => e.CourseClassId)
                .Distinct()
                .ToList();

            var query =
                from c in _db.CourseClasses
                join s in _db.Subjects on c.SubjectId equals s.Id into sj
                from subject in sj.DefaultIfEmpty()
                join l in _db.Lecturers on c.LecturerId equals l.Id into lj
                from lecturer in lj.DefaultIfEmpty()
                where enrolledClassIds.Contains(c.Id)
                select new CourseClassListViewModel
                {
                    Id = c.Id,
                    ClassCode = c.ClassCode,
                    SubjectName = subject != null ? subject.SubjectName : "",
                    SubjectCode = subject != null ? subject.SubjectCode : "",
                    LecturerName = lecturer != null ? lecturer.FullName : "",
                    Semester = c.Semester,
                    CurrentStudents = c.CurrentStudents,
                    MaxStudents = c.MaxStudents,
                    Room = c.Room,
                    Status = c.Status.ToString()
                };

            return query
                .AsNoTracking()
                .OrderByDescending(x => x.Semester)
                .ThenBy(x => x.ClassCode)
                .ToList();
        }

        public bool Create(CourseClassFormViewModel model)
        {
            // Check ClassCode trùng
            var codeLower = model.ClassCode.Trim().ToLower();
            if (_db.CourseClasses.Any(c => c.ClassCode.ToLower() == codeLower))
                return false;

            var courseClass = new CourseClass
            {
                // KHÔNG set Id nếu Id là Identity (SQL tự tăng)
                ClassCode = model.ClassCode.Trim(),
                SubjectId = model.SubjectId,
                LecturerId = model.LecturerId,
                Semester = model.Semester,
                MaxStudents = model.MaxStudents,
                CurrentStudents = 0,
                Room = model.Room,
                Status = model.Status,
                CreatedDate = DateTime.Now
            };

            _db.CourseClasses.Add(courseClass);
            _db.SaveChanges();
            return true;
        }

        public bool Update(CourseClassFormViewModel model)
        {
            var id = model.Id ?? 0;
            var courseClass = _db.CourseClasses.FirstOrDefault(c => c.Id == id);
            if (courseClass == null) return false;

            // Check ClassCode trùng (ngoại trừ chính nó)
            var codeLower = model.ClassCode.Trim().ToLower();
            if (_db.CourseClasses.Any(c => c.Id != courseClass.Id && c.ClassCode.ToLower() == codeLower))
                return false;

            courseClass.ClassCode = model.ClassCode.Trim();
            courseClass.SubjectId = model.SubjectId;
            courseClass.LecturerId = model.LecturerId;
            courseClass.Semester = model.Semester;
            courseClass.MaxStudents = model.MaxStudents;
            courseClass.Room = model.Room;
            courseClass.Status = model.Status;

            _db.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var courseClass = _db.CourseClasses.FirstOrDefault(c => c.Id == id);
            if (courseClass == null) return false;

            // Có enrollment thì không cho xoá
            if (_db.Enrollments.Any(e => e.CourseClassId == id))
                return false;

            // (tuỳ chọn) nếu muốn chặt hơn: check schedules/grades...
            // if (_db.Schedules.Any(s => s.CourseClassId == id)) return false;

            _db.CourseClasses.Remove(courseClass);
            _db.SaveChanges();
            return true;
        }
    }
}
