using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    public interface IStudentService
    {
        List<StudentListViewModel> GetAll();
        StudentDetailViewModel? GetDetailById(int id);
        Student? GetById(int id);
        Student? GetByUserId(int userId);
        bool Create(StudentFormViewModel model);
        bool Update(StudentFormViewModel model);
        bool Delete(int id);
        double? CalculateGPA(int studentId, string? semester = null);
        List<AdministrativeClass> GetAdministrativeClasses();
    }

    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _db;

        public StudentService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<StudentListViewModel> GetAll()
        {
            return _db.Students
                .Select(s => new StudentListViewModel
                {
                    Id = s.Id,
                    StudentCode = s.StudentCode,
                    FullName = s.FullName,
                    Email = s.Email,
                    Major = s.Major,
                    AdmissionYear = s.AdmissionYear,
                    AdministrativeClassName = s.AdministrativeClassId != null
                        ? _db.AdministrativeClasses
                            .Where(c => c.Id == s.AdministrativeClassId)
                            .Select(c => c.ClassName)
                            .FirstOrDefault()
                        : null
                })
                .OrderBy(s => s.StudentCode)
                .ToList();
        }

        public Student? GetById(int id) => _db.Students.FirstOrDefault(s => s.Id == id);

        public Student? GetByUserId(int userId) => _db.Students.FirstOrDefault(s => s.UserId == userId);

        public StudentDetailViewModel? GetDetailById(int id)
        {
            var student = GetById(id);
            if (student == null) return null;

            var enrollments = _db.Enrollments
                .Where(e => e.StudentId == id && e.Status == EnrollmentStatus.Approved)
                .ToList();

            var enrollmentVMs = enrollments.Select(e =>
            {
                var courseClass = _db.CourseClasses.FirstOrDefault(c => c.Id == e.CourseClassId);
                var subject = courseClass == null ? null : _db.Subjects.FirstOrDefault(s => s.Id == courseClass.SubjectId);
                var grade = _db.Grades.FirstOrDefault(g => g.EnrollmentId == e.Id);

                return new EnrollmentInfoViewModel
                {
                    SubjectName = subject?.SubjectName ?? "",
                    ClassCode = courseClass?.ClassCode ?? "",
                    Semester = courseClass?.Semester ?? "",
                    Status = e.Status.ToString(),
                    TotalScore = grade?.TotalScore,
                    LetterGrade = grade?.LetterGrade
                };
            }).ToList();

            return new StudentDetailViewModel
            {
                Id = student.Id,
                StudentCode = student.StudentCode,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth,
                PhoneNumber = student.PhoneNumber,
                Address = student.Address,
                Major = student.Major,
                AdmissionYear = student.AdmissionYear,
                AdministrativeClassName = student.AdministrativeClassId != null
                    ? _db.AdministrativeClasses
                        .Where(c => c.Id == student.AdministrativeClassId)
                        .Select(c => c.ClassName)
                        .FirstOrDefault()
                    : null,
                Enrollments = enrollmentVMs,
                GPA = CalculateGPA(id)
            };
        }

        public bool Create(StudentFormViewModel model)
        {
            if (_db.Students.Any(s => s.Email.ToLower() == model.Email.ToLower())) return false;
            if (_db.Students.Any(s => s.StudentCode.ToLower() == model.StudentCode.ToLower())) return false;

            var user = new User
            {
                Email = model.Email,
                Password = model.Password ?? "123456",
                FullName = model.FullName,
                Role = UserRole.Student,
                Status = UserStatus.Active,
                CreatedDate = DateTime.Now
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            var student = new Student
            {
                UserId = user.Id,
                StudentCode = model.StudentCode,
                FullName = model.FullName,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                AdministrativeClassId = model.AdministrativeClassId,
                Major = model.Major,
                AdmissionYear = model.AdmissionYear,
                CreatedDate = DateTime.Now
            };

            _db.Students.Add(student);
            _db.SaveChanges();
            return true;
        }

        public bool Update(StudentFormViewModel model)
        {
            var student = GetById(model.Id ?? 0);
            if (student == null) return false;

            if (_db.Students.Any(s => s.Id != student.Id && s.Email.ToLower() == model.Email.ToLower())) return false;
            if (_db.Students.Any(s => s.Id != student.Id && s.StudentCode.ToLower() == model.StudentCode.ToLower())) return false;

            student.StudentCode = model.StudentCode;
            student.FullName = model.FullName;
            student.Email = model.Email;
            student.DateOfBirth = model.DateOfBirth;
            student.PhoneNumber = model.PhoneNumber;
            student.Address = model.Address;
            student.Major = model.Major;
            student.AdmissionYear = model.AdmissionYear;
            student.AdministrativeClassId = model.AdministrativeClassId;

            var user = _db.Users.FirstOrDefault(u => u.Id == student.UserId);
            if (user != null)
            {
                user.Email = model.Email;
                user.FullName = model.FullName;
            }

            _db.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var student = GetById(id);
            if (student == null) return false;

            var user = _db.Users.FirstOrDefault(u => u.Id == student.UserId);
            if (user != null) _db.Users.Remove(user);

            _db.Students.Remove(student);
            _db.SaveChanges();
            return true;
        }

        public double? CalculateGPA(int studentId, string? semester = null)
        {
            var enrollments = _db.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved)
                .ToList();

            if (semester != null)
            {
                var classIds = _db.CourseClasses.Where(c => c.Semester == semester).Select(c => c.Id).ToList();
                enrollments = enrollments.Where(e => classIds.Contains(e.CourseClassId)).ToList();
            }

            var enrollmentIds = enrollments.Select(e => e.Id).ToList();
            var grades = _db.Grades.Where(g => enrollmentIds.Contains(g.EnrollmentId) && g.TotalScore.HasValue).ToList();
            if (!grades.Any()) return null;

            double total = 0;
            int credits = 0;

            foreach (var g in grades)
            {
                var enrollment = enrollments.First(e => e.Id == g.EnrollmentId);
                var subjectId = _db.CourseClasses.Where(c => c.Id == enrollment.CourseClassId).Select(c => c.SubjectId).FirstOrDefault();
                var subject = _db.Subjects.FirstOrDefault(s => s.Id == subjectId);
                if (subject == null) continue;

                total += g.TotalScore!.Value * subject.Credits;
                credits += subject.Credits;
            }

            return credits > 0 ? Math.Round(total / credits, 2) : null;
        }

        public List<AdministrativeClass> GetAdministrativeClasses() => _db.AdministrativeClasses.ToList();
    }
}
