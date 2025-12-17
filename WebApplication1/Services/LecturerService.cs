using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service qu?n lý Lecturer
    /// </summary>
    public interface ILecturerService
    {
        List<LecturerListViewModel> GetAll();
        LecturerDetailViewModel? GetDetailById(int id);
        Lecturer? GetById(int id);
        Lecturer? GetByUserId(int userId);
        bool Create(LecturerFormViewModel model);
        bool Update(LecturerFormViewModel model);
        bool Delete(int id);
    }

    public class LecturerService : ILecturerService
    {
        public List<LecturerListViewModel> GetAll()
        {
            return FakeDatabase.Lecturers.Select(l => new LecturerListViewModel
            {
                Id = l.Id,
                LecturerCode = l.LecturerCode,
                FullName = l.FullName,
                Email = l.Email,
                Department = l.Department,
                Title = l.Title,
                Specialization = l.Specialization
            }).OrderBy(l => l.LecturerCode).ToList();
        }

        public LecturerDetailViewModel? GetDetailById(int id)
        {
            var lecturer = GetById(id);
            if (lecturer == null) return null;

            var teachingClasses = FakeDatabase.CourseClasses
                .Where(c => c.LecturerId == id)
                .Select(c =>
                {
                    var subject = FakeDatabase.Subjects.FirstOrDefault(s => s.Id == c.SubjectId);
                    return new CourseClassInfoViewModel
                    {
                        Id = c.Id,
                        ClassCode = c.ClassCode,
                        SubjectName = subject?.SubjectName ?? "",
                        Semester = c.Semester,
                        CurrentStudents = c.CurrentStudents,
                        MaxStudents = c.MaxStudents,
                        Status = c.Status.ToString()
                    };
                }).ToList();

            return new LecturerDetailViewModel
            {
                Id = lecturer.Id,
                LecturerCode = lecturer.LecturerCode,
                FullName = lecturer.FullName,
                Email = lecturer.Email,
                DateOfBirth = lecturer.DateOfBirth,
                PhoneNumber = lecturer.PhoneNumber,
                Department = lecturer.Department,
                Title = lecturer.Title,
                Specialization = lecturer.Specialization,
                JoinDate = lecturer.JoinDate,
                TeachingClasses = teachingClasses
            };
        }

        public Lecturer? GetById(int id)
        {
            return FakeDatabase.Lecturers.FirstOrDefault(l => l.Id == id);
        }

        public Lecturer? GetByUserId(int userId)
        {
            return FakeDatabase.Lecturers.FirstOrDefault(l => l.UserId == userId);
        }

        public bool Create(LecturerFormViewModel model)
        {
            // Ki?m tra email và mã gi?ng viên ?ã t?n t?i
            if (FakeDatabase.Lecturers.Any(l => l.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            if (FakeDatabase.Lecturers.Any(l => l.LecturerCode.Equals(model.LecturerCode, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            // T?o User tr??c
            var user = new User
            {
                Id = FakeDatabase.GetNextUserId(),
                Email = model.Email,
                Password = model.Password ?? "123456",
                FullName = model.FullName,
                Role = UserRole.Lecturer,
                Status = UserStatus.Active,
                CreatedDate = DateTime.Now
            };
            FakeDatabase.Users.Add(user);

            // T?o Lecturer
            var lecturer = new Lecturer
            {
                Id = FakeDatabase.GetNextLecturerId(),
                UserId = user.Id,
                LecturerCode = model.LecturerCode,
                FullName = model.FullName,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                PhoneNumber = model.PhoneNumber,
                Department = model.Department,
                Title = model.Title,
                Specialization = model.Specialization,
                JoinDate = model.JoinDate
            };

            FakeDatabase.Lecturers.Add(lecturer);
            return true;
        }

        public bool Update(LecturerFormViewModel model)
        {
            var lecturer = GetById(model.Id ?? 0);
            if (lecturer == null) return false;

            // Ki?m tra email và mã gi?ng viên trùng
            if (FakeDatabase.Lecturers.Any(l => l.Id != lecturer.Id &&
                l.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            if (FakeDatabase.Lecturers.Any(l => l.Id != lecturer.Id &&
                l.LecturerCode.Equals(model.LecturerCode, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            lecturer.LecturerCode = model.LecturerCode;
            lecturer.FullName = model.FullName;
            lecturer.Email = model.Email;
            lecturer.DateOfBirth = model.DateOfBirth;
            lecturer.PhoneNumber = model.PhoneNumber;
            lecturer.Department = model.Department;
            lecturer.Title = model.Title;
            lecturer.Specialization = model.Specialization;
            lecturer.JoinDate = model.JoinDate;

            // C?p nh?t User
            var user = FakeDatabase.Users.FirstOrDefault(u => u.Id == lecturer.UserId);
            if (user != null)
            {
                user.Email = model.Email;
                user.FullName = model.FullName;
            }

            return true;
        }

        public bool Delete(int id)
        {
            var lecturer = GetById(id);
            if (lecturer == null) return false;

            // Xóa User liên quan
            var user = FakeDatabase.Users.FirstOrDefault(u => u.Id == lecturer.UserId);
            if (user != null)
            {
                FakeDatabase.Users.Remove(user);
            }

            FakeDatabase.Lecturers.Remove(lecturer);
            return true;
        }
    }
}
