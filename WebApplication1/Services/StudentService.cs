using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service qu?n lý Student
    /// </summary>
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
        public List<StudentListViewModel> GetAll()
        {
            return FakeDatabase.Students.Select(s => new StudentListViewModel
            {
                Id = s.Id,
                StudentCode = s.StudentCode,
                FullName = s.FullName,
                Email = s.Email,
                Major = s.Major,
                AdmissionYear = s.AdmissionYear,
                AdministrativeClassName = s.AdministrativeClassId.HasValue
                    ? FakeDatabase.AdministrativeClasses.FirstOrDefault(c => c.Id == s.AdministrativeClassId)?.ClassName
                    : null
            }).OrderBy(s => s.StudentCode).ToList();
        }

        public StudentDetailViewModel? GetDetailById(int id)
        {
            var student = GetById(id);
            if (student == null) return null;

            var enrollments = FakeDatabase.Enrollments
                .Where(e => e.StudentId == id && e.Status == EnrollmentStatus.Approved)
                .Select(e =>
                {
                    var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == e.CourseClassId);
                    var subject = courseClass != null
                        ? FakeDatabase.Subjects.FirstOrDefault(s => s.Id == courseClass.SubjectId)
                        : null;
                    var grade = FakeDatabase.Grades.FirstOrDefault(g => g.EnrollmentId == e.Id);

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
                AdministrativeClassName = student.AdministrativeClassId.HasValue
                    ? FakeDatabase.AdministrativeClasses.FirstOrDefault(c => c.Id == student.AdministrativeClassId)?.ClassName
                    : null,
                Enrollments = enrollments,
                GPA = CalculateGPA(id)
            };
        }

        public Student? GetById(int id)
        {
            return FakeDatabase.Students.FirstOrDefault(s => s.Id == id);
        }

        public Student? GetByUserId(int userId)
        {
            return FakeDatabase.Students.FirstOrDefault(s => s.UserId == userId);
        }

        public bool Create(StudentFormViewModel model)
        {
            // Ki?m tra email và mã sinh viên ?ã t?n t?i
            if (FakeDatabase.Students.Any(s => s.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            if (FakeDatabase.Students.Any(s => s.StudentCode.Equals(model.StudentCode, StringComparison.OrdinalIgnoreCase)))
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
                Role = UserRole.Student,
                Status = UserStatus.Active,
                CreatedDate = DateTime.Now
            };
            FakeDatabase.Users.Add(user);

            // T?o Student
            var student = new Student
            {
                Id = FakeDatabase.GetNextStudentId(),
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

            FakeDatabase.Students.Add(student);
            return true;
        }

        public bool Update(StudentFormViewModel model)
        {
            var student = GetById(model.Id ?? 0);
            if (student == null) return false;

            // Ki?m tra email và mã sinh viên trùng
            if (FakeDatabase.Students.Any(s => s.Id != student.Id &&
                s.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            if (FakeDatabase.Students.Any(s => s.Id != student.Id &&
                s.StudentCode.Equals(model.StudentCode, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            student.StudentCode = model.StudentCode;
            student.FullName = model.FullName;
            student.Email = model.Email;
            student.DateOfBirth = model.DateOfBirth;
            student.PhoneNumber = model.PhoneNumber;
            student.Address = model.Address;
            student.AdministrativeClassId = model.AdministrativeClassId;
            student.Major = model.Major;
            student.AdmissionYear = model.AdmissionYear;

            // C?p nh?t User
            var user = FakeDatabase.Users.FirstOrDefault(u => u.Id == student.UserId);
            if (user != null)
            {
                user.Email = model.Email;
                user.FullName = model.FullName;
            }

            return true;
        }

        public bool Delete(int id)
        {
            var student = GetById(id);
            if (student == null) return false;

            // Xóa User liên quan
            var user = FakeDatabase.Users.FirstOrDefault(u => u.Id == student.UserId);
            if (user != null)
            {
                FakeDatabase.Users.Remove(user);
            }

            FakeDatabase.Students.Remove(student);
            return true;
        }

        public double? CalculateGPA(int studentId, string? semester = null)
        {
            var enrollments = FakeDatabase.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved)
                .ToList();

            if (semester != null)
            {
                var courseClassIds = FakeDatabase.CourseClasses
                    .Where(c => c.Semester == semester)
                    .Select(c => c.Id)
                    .ToList();
                enrollments = enrollments.Where(e => courseClassIds.Contains(e.CourseClassId)).ToList();
            }

            var grades = FakeDatabase.Grades
                .Where(g => enrollments.Select(e => e.Id).Contains(g.EnrollmentId) && g.TotalScore.HasValue)
                .ToList();

            if (!grades.Any()) return null;

            double totalPoints = 0;
            int totalCredits = 0;

            foreach (var grade in grades)
            {
                var enrollment = enrollments.FirstOrDefault(e => e.Id == grade.EnrollmentId);
                if (enrollment == null) continue;

                var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == enrollment.CourseClassId);
                if (courseClass == null) continue;

                var subject = FakeDatabase.Subjects.FirstOrDefault(s => s.Id == courseClass.SubjectId);
                if (subject == null) continue;

                totalPoints += grade.TotalScore!.Value * subject.Credits;
                totalCredits += subject.Credits;
            }

            return totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : null;
        }

        public List<AdministrativeClass> GetAdministrativeClasses()
        {
            return FakeDatabase.AdministrativeClasses.ToList();
        }
    }
}
