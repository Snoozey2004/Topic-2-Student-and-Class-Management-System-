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
            var today = DateTime.Today;

            // =========================
            // 1) USERS
            // =========================
            var admin = new User
            {
                Email = "admin@university.edu.vn",
                Password = "admin123",
                FullName = "System Administrator",
                Role = UserRole.Admin,
                Status = UserStatus.Active,
                CreatedDate = now.AddYears(-1)
            };

            var uLect1 = new User
            {
                Email = "nguyenvana@university.edu.vn",
                Password = "lecturer123",
                FullName = "Dr. Nguyen Van A",
                Role = UserRole.Lecturer,
                Status = UserStatus.Active,
                CreatedDate = now.AddMonths(-6)
            };
            var uLect2 = new User
            {
                Email = "tranthib@university.edu.vn",
                Password = "lecturer123",
                FullName = "Assoc. Prof. Tran Thi B",
                Role = UserRole.Lecturer,
                Status = UserStatus.Active,
                CreatedDate = now.AddMonths(-6)
            };
            var uLect3 = new User
            {
                Email = "levanc@university.edu.vn",
                Password = "lecturer123",
                FullName = "MSc. Le Van C",
                Role = UserRole.Lecturer,
                Status = UserStatus.Active,
                CreatedDate = now.AddMonths(-5)
            };

            var uStu1 = new User
            {
                Email = "phamvand@student.edu.vn",
                Password = "student123",
                FullName = "Pham Van D",
                Role = UserRole.Student,
                Status = UserStatus.Active,
                CreatedDate = now.AddMonths(-3)
            };
            var uStu2 = new User
            {
                Email = "hoangthie@student.edu.vn",
                Password = "student123",
                FullName = "Hoang Thi E",
                Role = UserRole.Student,
                Status = UserStatus.Active,
                CreatedDate = now.AddMonths(-3)
            };
            var uStu3 = new User
            {
                Email = "vuthif@student.edu.vn",
                Password = "student123",
                FullName = "Vu Thi F",
                Role = UserRole.Student,
                Status = UserStatus.Active,
                CreatedDate = now.AddMonths(-3)
            };
            var uStu4 = new User
            {
                Email = "nguyenvang@student.edu.vn",
                Password = "student123",
                FullName = "Nguyen Van G",
                Role = UserRole.Student,
                Status = UserStatus.Active,
                CreatedDate = now.AddMonths(-2)
            };
            var uStu5 = new User
            {
                Email = "tranthih@student.edu.vn",
                Password = "student123",
                FullName = "Tran Thi H",
                Role = UserRole.Student,
                Status = UserStatus.Active,
                CreatedDate = now.AddMonths(-2)
            };

            db.Users.AddRange(admin, uLect1, uLect2, uLect3, uStu1, uStu2, uStu3, uStu4, uStu5);
            await db.SaveChangesAsync();

            // =========================
            // 2) LECTURERS
            // =========================
            var lect1 = new Lecturer
            {
                UserId = uLect1.Id,
                LecturerCode = "GV001",
                FullName = uLect1.FullName,
                Email = uLect1.Email,
                DateOfBirth = new DateTime(1980, 3, 15),
                PhoneNumber = "0911234567",
                Department = "Information Technology",
                Title = "PhD",
                Specialization = "Artificial Intelligence",
                JoinDate = new DateTime(2010, 9, 1)
            };

            var lect2 = new Lecturer
            {
                UserId = uLect2.Id,
                LecturerCode = "GV002",
                FullName = uLect2.FullName,
                Email = uLect2.Email,
                DateOfBirth = new DateTime(1975, 7, 20),
                PhoneNumber = "0912345678",
                Department = "Information Technology",
                Title = "Associate Professor",
                Specialization = "Database",
                JoinDate = new DateTime(2005, 9, 1)
            };

            var lect3 = new Lecturer
            {
                UserId = uLect3.Id,
                LecturerCode = "GV003",
                FullName = uLect3.FullName,
                Email = uLect3.Email,
                DateOfBirth = new DateTime(1985, 12, 5),
                PhoneNumber = "0913456789",
                Department = "Business Administration",
                Title = "MSc",
                Specialization = "Marketing",
                JoinDate = new DateTime(2015, 9, 1)
            };

            db.Lecturers.AddRange(lect1, lect2, lect3);
            await db.SaveChangesAsync();

            // =========================
            // 3) ADMINISTRATIVE CLASSES
            // =========================
            var adminClass1 = new AdministrativeClass
            {
                ClassName = "CNTT-K17A",
                Major = "Information Technology",
                AdmissionYear = 2021,
                AdvisorLecturerId = lect1.Id,
                CreatedDate = now.AddYears(-3)
            };

            var adminClass2 = new AdministrativeClass
            {
                ClassName = "QTKD-K18A",
                Major = "Business Administration",
                AdmissionYear = 2022,
                AdvisorLecturerId = lect3.Id,
                CreatedDate = now.AddYears(-2)
            };

            db.AdministrativeClasses.AddRange(adminClass1, adminClass2);
            await db.SaveChangesAsync();

            // =========================
            // 4) STUDENTS
            // =========================
            var stu1 = new Student
            {
                UserId = uStu1.Id,
                StudentCode = "SV001",
                FullName = uStu1.FullName,
                Email = uStu1.Email,
                DateOfBirth = new DateTime(2003, 5, 15),
                PhoneNumber = "0901234567",
                Address = "123 Nguyen Hue, District 1, HCMC",
                AdministrativeClassId = adminClass1.Id,
                Major = "Information Technology",
                AdmissionYear = 2021,
                CreatedDate = now.AddMonths(-3)
            };

            var stu2 = new Student
            {
                UserId = uStu2.Id,
                StudentCode = "SV002",
                FullName = uStu2.FullName,
                Email = uStu2.Email,
                DateOfBirth = new DateTime(2003, 8, 20),
                PhoneNumber = "0902345678",
                Address = "456 Le Loi, District 1, HCMC",
                AdministrativeClassId = adminClass1.Id,
                Major = "Information Technology",
                AdmissionYear = 2021,
                CreatedDate = now.AddMonths(-3)
            };

            var stu3 = new Student
            {
                UserId = uStu3.Id,
                StudentCode = "SV003",
                FullName = uStu3.FullName,
                Email = uStu3.Email,
                DateOfBirth = new DateTime(2003, 3, 10),
                PhoneNumber = "0903456789",
                Address = "789 Tran Hung Dao, District 5, HCMC",
                AdministrativeClassId = adminClass1.Id,
                Major = "Information Technology",
                AdmissionYear = 2021,
                CreatedDate = now.AddMonths(-3)
            };

            var stu4 = new Student
            {
                UserId = uStu4.Id,
                StudentCode = "SV004",
                FullName = uStu4.FullName,
                Email = uStu4.Email,
                DateOfBirth = new DateTime(2004, 7, 25),
                PhoneNumber = "0904567890",
                Address = "321 Vo Van Tan, District 3, HCMC",
                AdministrativeClassId = adminClass2.Id,
                Major = "Business Administration",
                AdmissionYear = 2022,
                CreatedDate = now.AddMonths(-2)
            };

            var stu5 = new Student
            {
                UserId = uStu5.Id,
                StudentCode = "SV005",
                FullName = uStu5.FullName,
                Email = uStu5.Email,
                DateOfBirth = new DateTime(2004, 11, 30),
                PhoneNumber = "0905678901",
                Address = "654 Pasteur, District 1, HCMC",
                AdministrativeClassId = adminClass2.Id,
                Major = "Business Administration",
                AdmissionYear = 2022,
                CreatedDate = now.AddMonths(-2)
            };

            db.Students.AddRange(stu1, stu2, stu3, stu4, stu5);
            await db.SaveChangesAsync();

            // =========================
            // 5) SUBJECTS
            // =========================
            var sIT001 = new Subject
            {
                SubjectCode = "IT001",
                SubjectName = "Introduction to Programming",
                Credits = 4,
                Department = "Information Technology",
                Description = "Introduction to basic programming concepts",
                PrerequisiteSubjectIds = new List<int>(),
                CreatedDate = now.AddYears(-2)
            };

            var sIT002 = new Subject
            {
                SubjectCode = "IT002",
                SubjectName = "Data Structures & Algorithms",
                Credits = 4,
                Department = "Information Technology",
                Description = "Fundamental data structures and algorithms",
                // prerequisite sẽ set sau khi SaveChanges (cần Id)
                CreatedDate = now.AddYears(-2)
            };

            var sIT003 = new Subject
            {
                SubjectCode = "IT003",
                SubjectName = "Database Systems",
                Credits = 4,
                Department = "Information Technology",
                Description = "Design and management of database systems",
                CreatedDate = now.AddYears(-2)
            };

            var sIT004 = new Subject
            {
                SubjectCode = "IT004",
                SubjectName = "Web Programming",
                Credits = 4,
                Department = "Information Technology",
                Description = "Developing web applications with ASP.NET Core",
                CreatedDate = now.AddYears(-2)
            };

            var sBA001 = new Subject
            {
                SubjectCode = "BA001",
                SubjectName = "Principles of Management",
                Credits = 3,
                Department = "Business Administration",
                Description = "Basic principles of management",
                PrerequisiteSubjectIds = new List<int>(),
                CreatedDate = now.AddYears(-2)
            };

            var sBA002 = new Subject
            {
                SubjectCode = "BA002",
                SubjectName = "Introduction to Marketing",
                Credits = 3,
                Department = "Business Administration",
                Description = "Fundamental marketing concepts and strategies",
                CreatedDate = now.AddYears(-2)
            };

            db.Subjects.AddRange(sIT001, sIT002, sIT003, sIT004, sBA001, sBA002);
            await db.SaveChangesAsync();

        
            sIT002.PrerequisiteSubjectIds = new List<int> { sIT001.Id };
            sIT003.PrerequisiteSubjectIds = new List<int> { sIT001.Id };
            sIT004.PrerequisiteSubjectIds = new List<int> { sIT001.Id, sIT003.Id };
            sBA002.PrerequisiteSubjectIds = new List<int> { sBA001.Id };
            await db.SaveChangesAsync();

            // =========================
            // 6) COURSE CLASSES
            // =========================
            var cc1 = new CourseClass
            {
                ClassCode = "IT001.01",
                SubjectId = sIT001.Id,
                LecturerId = lect1.Id,
                Semester = "HK1-2024",
                MaxStudents = 50,
                CurrentStudents = 3,
                Room = "A101",
                Status = CourseClassStatus.InProgress,
                CreatedDate = now.AddMonths(-2)
            };

            var cc2 = new CourseClass
            {
                ClassCode = "IT002.01",
                SubjectId = sIT002.Id,
                LecturerId = lect1.Id,
                Semester = "HK1-2024",
                MaxStudents = 50,
                CurrentStudents = 2,
                Room = "A102",
                Status = CourseClassStatus.InProgress,
                CreatedDate = now.AddMonths(-2)
            };

            var cc3 = new CourseClass
            {
                ClassCode = "IT003.01",
                SubjectId = sIT003.Id,
                LecturerId = lect2.Id,
                Semester = "HK1-2024",
                MaxStudents = 45,
                CurrentStudents = 1,
                Room = "B201",
                Status = CourseClassStatus.InProgress,
                CreatedDate = now.AddMonths(-2)
            };

            var cc4 = new CourseClass
            {
                ClassCode = "IT004.01",
                SubjectId = sIT004.Id,
                LecturerId = lect2.Id,
                Semester = "HK1-2024",
                MaxStudents = 40,
                CurrentStudents = 0,
                Room = "Lab301",
                Status = CourseClassStatus.Open,
                CreatedDate = now.AddMonths(-1)
            };

            var cc5 = new CourseClass
            {
                ClassCode = "BA001.01",
                SubjectId = sBA001.Id,
                LecturerId = lect3.Id,
                Semester = "HK1-2024",
                MaxStudents = 60,
                CurrentStudents = 2,
                Room = "C301",
                Status = CourseClassStatus.InProgress,
                CreatedDate = now.AddMonths(-2)
            };

            var cc6 = new CourseClass
            {
                ClassCode = "BA002.01",
                SubjectId = sBA002.Id,
                LecturerId = lect3.Id,
                Semester = "HK1-2024",
                MaxStudents = 60,
                CurrentStudents = 0,
                Room = "C302",
                Status = CourseClassStatus.Open,
                CreatedDate = now.AddMonths(-1)
            };

            db.CourseClasses.AddRange(cc1, cc2, cc3, cc4, cc5, cc6);
            await db.SaveChangesAsync();

            // =========================
            // 7) SCHEDULES
            // =========================
            db.Schedules.AddRange(
                new Schedule
                {
                    CourseClassId = cc1.Id,
                    DayOfWeek = DayOfWeek.Monday,
                    Session = "Morning",
                    Period = "Period 1-3",
                    StartTime = "07:00",
                    EndTime = "09:30",
                    Room = "A101",
                    EffectiveDate = now.AddMonths(-2),
                    CreatedDate = now.AddMonths(-2)
                },
                new Schedule
                {
                    CourseClassId = cc1.Id,
                    DayOfWeek = DayOfWeek.Wednesday,
                    Session = "Morning",
                    Period = "Period 1-3",
                    StartTime = "07:00",
                    EndTime = "09:30",
                    Room = "A101",
                    EffectiveDate = now.AddMonths(-2),
                    CreatedDate = now.AddMonths(-2)
                },

                new Schedule
                {
                    CourseClassId = cc2.Id,
                    DayOfWeek = DayOfWeek.Tuesday,
                    Session = "Afternoon",
                    Period = "Period 7-9",
                    StartTime = "13:00",
                    EndTime = "15:30",
                    Room = "A102",
                    EffectiveDate = now.AddMonths(-2),
                    CreatedDate = now.AddMonths(-2)
                },
                new Schedule
                {
                    CourseClassId = cc2.Id,
                    DayOfWeek = DayOfWeek.Thursday,
                    Session = "Afternoon",
                    Period = "Period 7-9",
                    StartTime = "13:00",
                    EndTime = "15:30",
                    Room = "A102",
                    EffectiveDate = now.AddMonths(-2),
                    CreatedDate = now.AddMonths(-2)
                },

                new Schedule
                {
                    CourseClassId = cc3.Id,
                    DayOfWeek = DayOfWeek.Monday,
                    Session = "Morning",
                    Period = "Period 4-6",
                    StartTime = "09:45",
                    EndTime = "12:15",
                    Room = "B201",
                    EffectiveDate = now.AddMonths(-2),
                    CreatedDate = now.AddMonths(-2)
                },
                new Schedule
                {
                    CourseClassId = cc3.Id,
                    DayOfWeek = DayOfWeek.Friday,
                    Session = "Morning",
                    Period = "Period 1-3",
                    StartTime = "07:00",
                    EndTime = "09:30",
                    Room = "B201",
                    EffectiveDate = now.AddMonths(-2),
                    CreatedDate = now.AddMonths(-2)
                },

                new Schedule
                {
                    CourseClassId = cc4.Id,
                    DayOfWeek = DayOfWeek.Wednesday,
                    Session = "Afternoon",
                    Period = "Period 7-9",
                    StartTime = "13:00",
                    EndTime = "15:30",
                    Room = "Lab301",
                    EffectiveDate = now.AddMonths(-2),
                    CreatedDate = now.AddMonths(-2)
                },
                new Schedule
                {
                    CourseClassId = cc4.Id,
                    DayOfWeek = DayOfWeek.Thursday,
                    Session = "Morning",
                    Period = "Period 4-6",
                    StartTime = "09:45",
                    EndTime = "12:15",
                    Room = "Lab301",
                    EffectiveDate = now.AddMonths(-2),
                    CreatedDate = now.AddMonths(-2)
                },

                new Schedule
                {
                    CourseClassId = cc5.Id,
                    DayOfWeek = DayOfWeek.Monday,
                    Session = "Afternoon",
                    Period = "Period 7-9",
                    StartTime = "13:00",
                    EndTime = "15:30",
                    Room = "C301",
                    EffectiveDate = now.AddMonths(-2),
                    CreatedDate = now.AddMonths(-2)
                },
                new Schedule
                {
                    CourseClassId = cc5.Id,
                    DayOfWeek = DayOfWeek.Wednesday,
                    Session = "Morning",
                    Period = "Period 4-6",
                    StartTime = "09:45",
                    EndTime = "12:15",
                    Room = "C301",
                    EffectiveDate = now.AddMonths(-2),
                    CreatedDate = now.AddMonths(-2)
                },

                new Schedule
                {
                    CourseClassId = cc6.Id,
                    DayOfWeek = DayOfWeek.Tuesday,
                    Session = "Morning",
                    Period = "Period 1-3",
                    StartTime = "07:00",
                    EndTime = "09:30",
                    Room = "C302",
                    EffectiveDate = now.AddMonths(-2),
                    CreatedDate = now.AddMonths(-2)
                },
                new Schedule
                {
                    CourseClassId = cc6.Id,
                    DayOfWeek = DayOfWeek.Friday,
                    Session = "Afternoon",
                    Period = "Period 7-9",
                    StartTime = "13:00",
                    EndTime = "15:30",
                    Room = "C302",
                    EffectiveDate = now.AddMonths(-2),
                    CreatedDate = now.AddMonths(-2)
                }
            );
            await db.SaveChangesAsync();

            // =========================
            // 8) ENROLLMENTS
            // =========================
            var e1 = new Enrollment { StudentId = stu1.Id, CourseClassId = cc1.Id, EnrollmentDate = now.AddMonths(-2), Status = EnrollmentStatus.Approved, ApprovedDate = now.AddMonths(-2), ApprovedBy = admin.Id };
            var e2 = new Enrollment { StudentId = stu1.Id, CourseClassId = cc2.Id, EnrollmentDate = now.AddMonths(-2), Status = EnrollmentStatus.Approved, ApprovedDate = now.AddMonths(-2), ApprovedBy = admin.Id };

            var e3 = new Enrollment { StudentId = stu2.Id, CourseClassId = cc1.Id, EnrollmentDate = now.AddMonths(-2), Status = EnrollmentStatus.Approved, ApprovedDate = now.AddMonths(-2), ApprovedBy = admin.Id };
            var e4 = new Enrollment { StudentId = stu2.Id, CourseClassId = cc3.Id, EnrollmentDate = now.AddMonths(-2), Status = EnrollmentStatus.Approved, ApprovedDate = now.AddMonths(-2), ApprovedBy = admin.Id };

            var e5 = new Enrollment { StudentId = stu3.Id, CourseClassId = cc1.Id, EnrollmentDate = now.AddMonths(-2), Status = EnrollmentStatus.Approved, ApprovedDate = now.AddMonths(-2), ApprovedBy = admin.Id };
            var e6 = new Enrollment { StudentId = stu3.Id, CourseClassId = cc2.Id, EnrollmentDate = now.AddMonths(-1), Status = EnrollmentStatus.Pending };

            var e7 = new Enrollment { StudentId = stu4.Id, CourseClassId = cc5.Id, EnrollmentDate = now.AddMonths(-2), Status = EnrollmentStatus.Approved, ApprovedDate = now.AddMonths(-2), ApprovedBy = admin.Id };
            var e8 = new Enrollment { StudentId = stu5.Id, CourseClassId = cc5.Id, EnrollmentDate = now.AddMonths(-2), Status = EnrollmentStatus.Approved, ApprovedDate = now.AddMonths(-2), ApprovedBy = admin.Id };

            db.Enrollments.AddRange(e1, e2, e3, e4, e5, e6, e7, e8);
            await db.SaveChangesAsync();

            // =========================
            // 9) GRADES
            // =========================
            db.Grades.AddRange(
                new Grade
                {
                    EnrollmentId = e1.Id,
                    StudentId = stu1.Id,
                    CourseClassId = cc1.Id,
                    AttendanceScore = 9.0,
                    MidtermScore = 8.5,
                    FinalScore = 8.0,
                    TotalScore = 8.3,
                    LetterGrade = "B+",
                    IsPassed = true,
                    LastUpdated = now.AddDays(-10),
                    UpdatedBy = lect1.Id
                },
                new Grade
                {
                    EnrollmentId = e2.Id,
                    StudentId = stu1.Id,
                    CourseClassId = cc2.Id,
                    AttendanceScore = 8.5,
                    MidtermScore = 7.5,
                    FinalScore = null,
                    TotalScore = null,
                    LetterGrade = null,
                    IsPassed = false,
                    LastUpdated = now.AddDays(-5),
                    UpdatedBy = lect1.Id
                },
                new Grade
                {
                    EnrollmentId = e3.Id,
                    StudentId = stu2.Id,
                    CourseClassId = cc1.Id,
                    AttendanceScore = 10.0,
                    MidtermScore = 9.0,
                    FinalScore = 9.5,
                    TotalScore = 9.4,
                    LetterGrade = "A",
                    IsPassed = true,
                    LastUpdated = now.AddDays(-10),
                    UpdatedBy = lect1.Id
                },
                new Grade
                {
                    EnrollmentId = e7.Id,
                    StudentId = stu4.Id,
                    CourseClassId = cc5.Id,
                    AttendanceScore = 7.0,
                    MidtermScore = null,
                    FinalScore = null,
                    TotalScore = null,
                    LetterGrade = null,
                    IsPassed = false,
                    LastUpdated = now.AddDays(-3),
                    UpdatedBy = lect3.Id
                }
            );
            await db.SaveChangesAsync();

            // =========================
            // 10) ATTENDANCES
            // =========================
            db.Attendances.AddRange(
                new Attendance
                {
                    EnrollmentId = e1.Id,
                    StudentId = stu1.Id,
                    CourseClassId = cc1.Id,
                    AttendanceDate = today.AddDays(-7),
                    Session = "Period 1-3",
                    Status = AttendanceStatus.Present,
                    CreatedDate = today.AddDays(-7),
                    CreatedBy = lect1.Id
                },
                new Attendance
                {
                    EnrollmentId = e1.Id,
                    StudentId = stu1.Id,
                    CourseClassId = cc1.Id,
                    AttendanceDate = today.AddDays(-5),
                    Session = "Period 1-3",
                    Status = AttendanceStatus.Present,
                    CreatedDate = today.AddDays(-5),
                    CreatedBy = lect1.Id
                },
                new Attendance
                {
                    EnrollmentId = e3.Id,
                    StudentId = stu2.Id,
                    CourseClassId = cc1.Id,
                    AttendanceDate = today.AddDays(-7),
                    Session = "Period 1-3",
                    Status = AttendanceStatus.Present,
                    CreatedDate = today.AddDays(-7),
                    CreatedBy = lect1.Id
                },
                new Attendance
                {
                    EnrollmentId = e3.Id,
                    StudentId = stu2.Id,
                    CourseClassId = cc1.Id,
                    AttendanceDate = today.AddDays(-5),
                    Session = "Period 1-3",
                    Status = AttendanceStatus.Late,
                    Note = "Late by 10 minutes",
                    CreatedDate = today.AddDays(-5),
                    CreatedBy = lect1.Id
                }
            );
            await db.SaveChangesAsync();

            // =========================
            // 11) NOTIFICATIONS
            // =========================
            db.Notifications.AddRange(
                new Notification
                {
                    UserId = uStu1.Id,
                    Title = "Course registration successful",
                    Message = "You have successfully registered for IT001 - Introduction to Programming",
                    Type = NotificationType.Enrollment,
                    IsRead = true,
                    LinkUrl = "/Student/Enrollments",
                    CreatedDate = now.AddMonths(-2),
                    ReadDate = now.AddMonths(-2).AddHours(2)
                },
                new Notification
                {
                    UserId = uStu1.Id,
                    Title = "New grade available",
                    Message = "Grades for IT001 have been updated. View details on the Grades page.",
                    Type = NotificationType.Grade,
                    IsRead = false,
                    LinkUrl = "/Student/Grades",
                    CreatedDate = now.AddDays(-10)
                },
                new Notification
                {
                    UserId = uStu2.Id,
                    Title = "Schedule update",
                    Message = "Schedule for IT001 has been updated. Please check your timetable.",
                    Type = NotificationType.Schedule,
                    IsRead = false,
                    LinkUrl = "/Student/Schedule",
                    CreatedDate = now.AddDays(-3)
                },
                new Notification
                {
                    UserId = uStu3.Id,
                    Title = "Registration pending",
                    Message = "Your registration for IT002 is pending review.",
                    Type = NotificationType.Enrollment,
                    IsRead = false,
                    LinkUrl = "/Student/Enrollments",
                    CreatedDate = now.AddDays(-1)
                },
                new Notification
                {
                    UserId = uLect1.Id,
                    Title = "System Notice",
                    Message = "Please update final grades before 30/12/2024.",
                    Type = NotificationType.System,
                    IsRead = false,
                    CreatedDate = now.AddDays(-2)
                }
            );
            await db.SaveChangesAsync();
        }
    }
}
