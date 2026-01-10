using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            await db.Database.MigrateAsync();

            // đã có data thì thôi
            if (await db.Users.AnyAsync()) return;

            var now = DateTime.Now;

            // ===== USERS =====
            var adminUser = new User
            {
                Email = "admin@university.edu.vn",
                Password = "admin123",
                FullName = "System Administrator",
                Role = UserRole.Admin,
                Status = UserStatus.Active,
                CreatedDate = now.AddYears(-1)
            };

            var lecturerUsers = new[]
            {
                new User
                {
                    Email = "nguyenvana@university.edu.vn",
                    Password = "lecturer123",
                    FullName = "Nguyen Van A",
                    Role = UserRole.Lecturer,
                    Status = UserStatus.Active,
                    CreatedDate = now.AddMonths(-6)
                },
                new User
                {
                    Email = "tranthib@university.edu.vn",
                    Password = "lecturer123",
                    FullName = "Tran Thi B",
                    Role = UserRole.Lecturer,
                    Status = UserStatus.Active,
                    CreatedDate = now.AddMonths(-6)
                }
            };

            var studentUsers = new[]
            {
                new User
                {
                    Email = "phamvand@student.edu.vn",
                    Password = "student123",
                    FullName = "Pham Van D",
                    Role = UserRole.Student,
                    Status = UserStatus.Active,
                    CreatedDate = now.AddMonths(-3)
                },
                new User
                {
                    Email = "hoangthie@student.edu.vn",
                    Password = "student123",
                    FullName = "Hoang Thi E",
                    Role = UserRole.Student,
                    Status = UserStatus.Active,
                    CreatedDate = now.AddMonths(-3)
                }
            };

            db.Users.Add(adminUser);
            db.Users.AddRange(lecturerUsers);
            db.Users.AddRange(studentUsers);
            await db.SaveChangesAsync();

            // ===== LECTURERS & STUDENTS =====
            var lecturers = new[]
            {
                new Lecturer
                {
                    UserId = lecturerUsers[0].Id,
                    LecturerCode = "GV001",
                    FullName = "Nguyen Van A",
                    Email = lecturerUsers[0].Email,
                    DateOfBirth = now.AddYears(-35),
                    PhoneNumber = "0901000001",
                    Department = "Information Technology",
                    Title = "PhD",
                    Specialization = "Software Engineering",
                    JoinDate = now.AddYears(-5)
                },
                new Lecturer
                {
                    UserId = lecturerUsers[1].Id,
                    LecturerCode = "GV002",
                    FullName = "Tran Thi B",
                    Email = lecturerUsers[1].Email,
                    DateOfBirth = now.AddYears(-32),
                    PhoneNumber = "0902000002",
                    Department = "Information Technology",
                    Title = "MSc",
                    Specialization = "Database Systems",
                    JoinDate = now.AddYears(-4)
                }
            };

            var students = new[]
            {
                new Student
                {
                    UserId = studentUsers[0].Id,
                    StudentCode = "SV001",
                    FullName = "Pham Van D",
                    Email = studentUsers[0].Email,
                    DateOfBirth = now.AddYears(-20),
                    PhoneNumber = "0903000003",
                    Address = "Hanoi",
                    Major = "Software Engineering",
                    AdmissionYear = now.Year - 2,
                    CreatedDate = now.AddMonths(-20)
                },
                new Student
                {
                    UserId = studentUsers[1].Id,
                    StudentCode = "SV002",
                    FullName = "Hoang Thi E",
                    Email = studentUsers[1].Email,
                    DateOfBirth = now.AddYears(-21),
                    PhoneNumber = "0904000004",
                    Address = "Da Nang",
                    Major = "Information Systems",
                    AdmissionYear = now.Year - 2,
                    CreatedDate = now.AddMonths(-20)
                }
            };

            db.Lecturers.AddRange(lecturers);
            db.Students.AddRange(students);

            // ===== SUBJECTS =====
            var subjects = new[]
            {
                new Subject
                {
                    SubjectCode = "IT001",
                    SubjectName = "Programming Fundamentals",
                    Credits = 3,
                    Department = "Information Technology",
                    Description = "Introduction to programming with C#",
                    PrerequisiteSubjectIds = new List<int>(),
                    CreatedDate = now.AddYears(-1)
                },
                new Subject
                {
                    SubjectCode = "IT002",
                    SubjectName = "Database Systems",
                    Credits = 3,
                    Department = "Information Technology",
                    Description = "Relational databases and SQL",
                    PrerequisiteSubjectIds = new List<int>(),
                    CreatedDate = now.AddYears(-1)
                }
            };

            db.Subjects.AddRange(subjects);
            await db.SaveChangesAsync();

            // ===== COURSE CLASSES =====
            var courseClasses = new List<CourseClass>
            {
                new CourseClass
                {
                    ClassCode = "IT001-01",
                    SubjectId = subjects[0].Id,
                    LecturerId = lecturers[0].Id,
                    Semester = "HK1-2024",
                    MaxStudents = 40,
                    CurrentStudents = 0,
                    Room = "A101",
                    Status = CourseClassStatus.Open,
                    CreatedDate = now.AddMonths(-1)
                },
                new CourseClass
                {
                    ClassCode = "IT002-01",
                    SubjectId = subjects[1].Id,
                    LecturerId = lecturers[1].Id,
                    Semester = "HK1-2024",
                    MaxStudents = 40,
                    CurrentStudents = 0,
                    Room = "B201",
                    Status = CourseClassStatus.Open,
                    CreatedDate = now.AddMonths(-1)
                }
            };

            db.CourseClasses.AddRange(courseClasses);
            await db.SaveChangesAsync();

            // ===== SCHEDULES =====
            var schedules = new[]
            {
                new Schedule
                {
                    CourseClassId = courseClasses[0].Id,
                    DayOfWeek = DayOfWeek.Monday,
                    Session = "Morning",
                    Period = "Period 1-3",
                    StartTime = "07:00",
                    EndTime = "09:30",
                    Room = "A101",
                    EffectiveDate = now.Date.AddDays(-7),
                    CreatedDate = now.AddDays(-7)
                },
                new Schedule
                {
                    CourseClassId = courseClasses[1].Id,
                    DayOfWeek = DayOfWeek.Wednesday,
                    Session = "Morning",
                    Period = "Period 1-3",
                    StartTime = "07:00",
                    EndTime = "09:30",
                    Room = "B201",
                    EffectiveDate = now.Date.AddDays(-7),
                    CreatedDate = now.AddDays(-7)
                }
            };

            db.Schedules.AddRange(schedules);
            await db.SaveChangesAsync();

            // ===== ENROLLMENTS =====
            var enrollments = new List<Enrollment>
            {
                new Enrollment
                {
                    StudentId = students[0].Id,
                    CourseClassId = courseClasses[0].Id,
                    EnrollmentDate = now.AddDays(-20),
                    Status = EnrollmentStatus.Approved,
                    ApprovedDate = now.AddDays(-19),
                    ApprovedBy = adminUser.Id
                },
                new Enrollment
                {
                    StudentId = students[1].Id,
                    CourseClassId = courseClasses[1].Id,
                    EnrollmentDate = now.AddDays(-18),
                    Status = EnrollmentStatus.Approved,
                    ApprovedDate = now.AddDays(-17),
                    ApprovedBy = adminUser.Id
                }
            };

            db.Enrollments.AddRange(enrollments);

            courseClasses[0].CurrentStudents = enrollments.Count(e => e.CourseClassId == courseClasses[0].Id);
            courseClasses[1].CurrentStudents = enrollments.Count(e => e.CourseClassId == courseClasses[1].Id);

            await db.SaveChangesAsync();
        }
    }
}
