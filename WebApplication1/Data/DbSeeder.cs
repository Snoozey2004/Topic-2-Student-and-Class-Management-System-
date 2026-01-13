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
            var rng = new Random(1234);

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
                },
                new User
                {
                    Email = "lethiF@student.edu.vn",
                    Password = "student123",
                    FullName = "Le Thi F",
                    Role = UserRole.Student,
                    Status = UserStatus.Active,
                    CreatedDate = now.AddMonths(-2)
                },
                new User
                {
                    Email = "tranvanG@student.edu.vn",
                    Password = "student123",
                    FullName = "Tran Van G",
                    Role = UserRole.Student,
                    Status = UserStatus.Active,
                    CreatedDate = now.AddMonths(-2)
                },
                new User
                {
                    Email = "nguyenthih@student.edu.vn",
                    Password = "student123",
                    FullName = "Nguyen Thi H",
                    Role = UserRole.Student,
                    Status = UserStatus.Active,
                    CreatedDate = now.AddMonths(-2)
                },
                new User
                {
                    Email = "dangvant@student.edu.vn",
                    Password = "student123",
                    FullName = "Dang Van T",
                    Role = UserRole.Student,
                    Status = UserStatus.Active,
                    CreatedDate = now.AddMonths(-1)
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
                },
                new Student
                {
                    UserId = studentUsers[2].Id,
                    StudentCode = "SV003",
                    FullName = "Le Thi F",
                    Email = studentUsers[2].Email,
                    DateOfBirth = now.AddYears(-20),
                    PhoneNumber = "0905000005",
                    Address = "Hai Phong",
                    Major = "Software Engineering",
                    AdmissionYear = now.Year - 2,
                    CreatedDate = now.AddMonths(-18)
                },
                new Student
                {
                    UserId = studentUsers[3].Id,
                    StudentCode = "SV004",
                    FullName = "Tran Van G",
                    Email = studentUsers[3].Email,
                    DateOfBirth = now.AddYears(-22),
                    PhoneNumber = "0906000006",
                    Address = "HCMC",
                    Major = "Information Systems",
                    AdmissionYear = now.Year - 3,
                    CreatedDate = now.AddMonths(-18)
                },
                new Student
                {
                    UserId = studentUsers[4].Id,
                    StudentCode = "SV005",
                    FullName = "Nguyen Thi H",
                    Email = studentUsers[4].Email,
                    DateOfBirth = now.AddYears(-21),
                    PhoneNumber = "0907000007",
                    Address = "Can Tho",
                    Major = "Data Science",
                    AdmissionYear = now.Year - 2,
                    CreatedDate = now.AddMonths(-16)
                },
                new Student
                {
                    UserId = studentUsers[5].Id,
                    StudentCode = "SV006",
                    FullName = "Dang Van T",
                    Email = studentUsers[5].Email,
                    DateOfBirth = now.AddYears(-23),
                    PhoneNumber = "0908000008",
                    Address = "Hue",
                    Major = "Cybersecurity",
                    AdmissionYear = now.Year - 3,
                    CreatedDate = now.AddMonths(-15)
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
                },
                new Subject
                {
                    SubjectCode = "IT003",
                    SubjectName = "Data Structures",
                    Credits = 4,
                    Department = "Information Technology",
                    Description = "Algorithms and data structures in practice",
                    PrerequisiteSubjectIds = new List<int>(),
                    CreatedDate = now.AddYears(-1)
                },
                new Subject
                {
                    SubjectCode = "IT004",
                    SubjectName = "Web Development",
                    Credits = 3,
                    Department = "Information Technology",
                    Description = "Building web applications with ASP.NET Core",
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
                    ClassCode = "IT001-02",
                    SubjectId = subjects[0].Id,
                    LecturerId = lecturers[1].Id,
                    Semester = "HK1-2024",
                    MaxStudents = 40,
                    CurrentStudents = 0,
                    Room = "A102",
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
                },
                new CourseClass
                {
                    ClassCode = "IT002-02",
                    SubjectId = subjects[1].Id,
                    LecturerId = lecturers[0].Id,
                    Semester = "HK2-2024",
                    MaxStudents = 40,
                    CurrentStudents = 0,
                    Room = "B202",
                    Status = CourseClassStatus.Open,
                    CreatedDate = now.AddMonths(-1)
                },
                new CourseClass
                {
                    ClassCode = "IT003-01",
                    SubjectId = subjects[2].Id,
                    LecturerId = lecturers[0].Id,
                    Semester = "HK2-2024",
                    MaxStudents = 50,
                    CurrentStudents = 0,
                    Room = "C301",
                    Status = CourseClassStatus.Open,
                    CreatedDate = now.AddMonths(-1)
                },
                new CourseClass
                {
                    ClassCode = "IT004-01",
                    SubjectId = subjects[3].Id,
                    LecturerId = lecturers[1].Id,
                    Semester = "HK2-2024",
                    MaxStudents = 45,
                    CurrentStudents = 0,
                    Room = "Lab301",
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
                    CourseClassId = courseClasses[2].Id,
                    DayOfWeek = DayOfWeek.Wednesday,
                    Session = "Morning",
                    Period = "Period 1-3",
                    StartTime = "07:00",
                    EndTime = "09:30",
                    Room = "B201",
                    EffectiveDate = now.Date.AddDays(-7),
                    CreatedDate = now.AddDays(-7)
                },
                new Schedule
                {
                    CourseClassId = courseClasses[1].Id,
                    DayOfWeek = DayOfWeek.Tuesday,
                    Session = "Afternoon",
                    Period = "Period 7-9",
                    StartTime = "13:00",
                    EndTime = "15:30",
                    Room = "A102",
                    EffectiveDate = now.Date.AddDays(-7),
                    CreatedDate = now.AddDays(-7)
                },
                new Schedule
                {
                    CourseClassId = courseClasses[3].Id,
                    DayOfWeek = DayOfWeek.Thursday,
                    Session = "Evening",
                    Period = "Period 10-12",
                    StartTime = "15:45",
                    EndTime = "18:15",
                    Room = "B202",
                    EffectiveDate = now.Date.AddDays(-7),
                    CreatedDate = now.AddDays(-7)
                },
                new Schedule
                {
                    CourseClassId = courseClasses[4].Id,
                    DayOfWeek = DayOfWeek.Friday,
                    Session = "Evening",
                    Period = "Period 13-15",
                    StartTime = "18:30",
                    EndTime = "21:00",
                    Room = "C301",
                    EffectiveDate = now.Date.AddDays(-7),
                    CreatedDate = now.AddDays(-7)
                },
                new Schedule
                {
                    CourseClassId = courseClasses[5].Id,
                    DayOfWeek = DayOfWeek.Saturday,
                    Session = "Afternoon",
                    Period = "Period 4-6",
                    StartTime = "09:45",
                    EndTime = "12:15",
                    Room = "Lab301",
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
                    CourseClassId = courseClasses[2].Id,
                    EnrollmentDate = now.AddDays(-18),
                    Status = EnrollmentStatus.Approved,
                    ApprovedDate = now.AddDays(-17),
                    ApprovedBy = adminUser.Id
                },
                new Enrollment
                {
                    StudentId = students[2].Id,
                    CourseClassId = courseClasses[1].Id,
                    EnrollmentDate = now.AddDays(-15),
                    Status = EnrollmentStatus.Approved,
                    ApprovedDate = now.AddDays(-14),
                    ApprovedBy = adminUser.Id
                },
                new Enrollment
                {
                    StudentId = students[3].Id,
                    CourseClassId = courseClasses[1].Id,
                    EnrollmentDate = now.AddDays(-15),
                    Status = EnrollmentStatus.Approved,
                    ApprovedDate = now.AddDays(-14),
                    ApprovedBy = adminUser.Id
                },
                new Enrollment
                {
                    StudentId = students[4].Id,
                    CourseClassId = courseClasses[3].Id,
                    EnrollmentDate = now.AddDays(-12),
                    Status = EnrollmentStatus.Approved,
                    ApprovedDate = now.AddDays(-11),
                    ApprovedBy = adminUser.Id
                },
                new Enrollment
                {
                    StudentId = students[5].Id,
                    CourseClassId = courseClasses[4].Id,
                    EnrollmentDate = now.AddDays(-10),
                    Status = EnrollmentStatus.Approved,
                    ApprovedDate = now.AddDays(-9),
                    ApprovedBy = adminUser.Id
                }
            };

            db.Enrollments.AddRange(enrollments);
            await db.SaveChangesAsync();

            // Update current students counts
            foreach (var cc in courseClasses)
            {
                cc.CurrentStudents = enrollments.Count(e => e.CourseClassId == cc.Id);
            }
            await db.SaveChangesAsync();

            // ===== GRADES =====
            var grades = new List<Grade>();
            foreach (var enrollment in enrollments)
            {
                double attendanceScore = Math.Round(4 + rng.NextDouble() * 6, 2); // 4-10
                double midtermScore = Math.Round(3 + rng.NextDouble() * 7, 2);    // 3-10
                double finalScore = Math.Round(2 + rng.NextDouble() * 8, 2);      // 2-10
                var totalScore = Math.Round(0.1 * attendanceScore + 0.3 * midtermScore + 0.6 * finalScore, 2);
                var letter = GetLetterGrade(totalScore);
                var passed = totalScore >= 4.0;

                grades.Add(new Grade
                {
                    EnrollmentId = enrollment.Id,
                    StudentId = enrollment.StudentId,
                    CourseClassId = enrollment.CourseClassId,
                    AttendanceScore = attendanceScore,
                    MidtermScore = midtermScore,
                    FinalScore = finalScore,
                    TotalScore = totalScore,
                    LetterGrade = letter,
                    IsPassed = passed,
                    LastUpdated = now,
                    UpdatedBy = lecturers[0].Id
                });
            }

            db.Grades.AddRange(grades);
            await db.SaveChangesAsync();
        }

        private static string GetLetterGrade(double totalScore)
        {
            if (totalScore >= 8.5) return "A";
            if (totalScore >= 8.0) return "B+";
            if (totalScore >= 7.0) return "B";
            if (totalScore >= 6.5) return "C+";
            if (totalScore >= 5.5) return "C";
            if (totalScore >= 5.0) return "D+";
            if (totalScore >= 4.0) return "D";
            return "F";
        }
    }
}
