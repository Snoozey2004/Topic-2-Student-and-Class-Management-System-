using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service quản lý Lecturer (SQL Server + EF Core)
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
        private readonly ApplicationDbContext _db;

        public LecturerService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<LecturerListViewModel> GetAll()
        {
            return _db.Lecturers
                .AsNoTracking()
                .Select(l => new LecturerListViewModel
                {
                    Id = l.Id,
                    LecturerCode = l.LecturerCode,
                    FullName = l.FullName,
                    Email = l.Email,
                    Department = l.Department,
                    Title = l.Title,
                    Specialization = l.Specialization
                })
                .OrderBy(l => l.LecturerCode)
                .ToList();
        }

        public LecturerDetailViewModel? GetDetailById(int id)
        {
            var lecturer = _db.Lecturers
                .AsNoTracking()
                .FirstOrDefault(l => l.Id == id);

            if (lecturer == null) return null;

            // Teaching classes + subject name
            var teachingClasses =
                (from c in _db.CourseClasses.AsNoTracking()
                 join s in _db.Subjects.AsNoTracking() on c.SubjectId equals s.Id into sj
                 from subject in sj.DefaultIfEmpty()
                 where c.LecturerId == id
                 select new CourseClassInfoViewModel
                 {
                     Id = c.Id,
                     ClassCode = c.ClassCode,
                     SubjectName = subject != null ? subject.SubjectName : "",
                     Semester = c.Semester,
                     CurrentStudents = c.CurrentStudents,
                     MaxStudents = c.MaxStudents,
                     Status = c.Status.ToString()
                 })
                .ToList();

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
            return _db.Lecturers.FirstOrDefault(l => l.Id == id);
        }

        public Lecturer? GetByUserId(int userId)
        {
            return _db.Lecturers.FirstOrDefault(l => l.UserId == userId);
        }

        public bool Create(LecturerFormViewModel model)
        {
            var emailLower = model.Email.Trim().ToLower();
            var codeLower = model.LecturerCode.Trim().ToLower();

            // Check trùng email / lecturer code
            if (_db.Lecturers.Any(l => l.Email.ToLower() == emailLower))
                return false;

            if (_db.Lecturers.Any(l => l.LecturerCode.ToLower() == codeLower))
                return false;

            using var tx = _db.Database.BeginTransaction();

            // Tạo User trước
            var user = new User
            {
                // KHÔNG set Id nếu Identity
                Email = model.Email.Trim(),
                Password = string.IsNullOrWhiteSpace(model.Password) ? "123456" : model.Password!,
                FullName = model.FullName,
                Role = UserRole.Lecturer,
                Status = UserStatus.Active,
                CreatedDate = DateTime.Now
            };

            _db.Users.Add(user);
            _db.SaveChanges(); // để có user.Id

            // Tạo Lecturer
            var lecturer = new Lecturer
            {
                // KHÔNG set Id nếu Identity
                UserId = user.Id,
                LecturerCode = model.LecturerCode.Trim(),
                FullName = model.FullName,
                Email = model.Email.Trim(),
                DateOfBirth = model.DateOfBirth,
                PhoneNumber = model.PhoneNumber,
                Department = model.Department,
                Title = model.Title,
                Specialization = model.Specialization,
                JoinDate = model.JoinDate
            };

            _db.Lecturers.Add(lecturer);
            _db.SaveChanges();

            tx.Commit();
            return true;
        }

        public bool Update(LecturerFormViewModel model)
        {
            var id = model.Id ?? 0;
            var lecturer = _db.Lecturers.FirstOrDefault(l => l.Id == id);
            if (lecturer == null) return false;

            var emailLower = model.Email.Trim().ToLower();
            var codeLower = model.LecturerCode.Trim().ToLower();

            // Check trùng email / code (ngoại trừ chính nó)
            if (_db.Lecturers.Any(l => l.Id != lecturer.Id && l.Email.ToLower() == emailLower))
                return false;

            if (_db.Lecturers.Any(l => l.Id != lecturer.Id && l.LecturerCode.ToLower() == codeLower))
                return false;

            lecturer.LecturerCode = model.LecturerCode.Trim();
            lecturer.FullName = model.FullName;
            lecturer.Email = model.Email.Trim();
            lecturer.DateOfBirth = model.DateOfBirth;
            lecturer.PhoneNumber = model.PhoneNumber;
            lecturer.Department = model.Department;
            lecturer.Title = model.Title;
            lecturer.Specialization = model.Specialization;
            lecturer.JoinDate = model.JoinDate;

            // Update User liên quan
            var user = _db.Users.FirstOrDefault(u => u.Id == lecturer.UserId);
            if (user != null)
            {
                user.Email = model.Email.Trim();
                user.FullName = model.FullName;
            }

            _db.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var lecturer = _db.Lecturers.FirstOrDefault(l => l.Id == id);
            if (lecturer == null) return false;

            using var tx = _db.Database.BeginTransaction();

            // (tuỳ chọn) không cho xoá nếu đang có lớp dạy
            // if (_db.CourseClasses.Any(c => c.LecturerId == id)) return false;

            // Xóa User liên quan
            var user = _db.Users.FirstOrDefault(u => u.Id == lecturer.UserId);
            if (user != null)
            {
                _db.Users.Remove(user);
            }

            _db.Lecturers.Remove(lecturer);
            _db.SaveChanges();

            tx.Commit();
            return true;
        }
    }
}
