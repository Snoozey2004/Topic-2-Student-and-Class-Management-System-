using WebApplication1.Models;

namespace WebApplication1.Data
{
    /// <summary>
    /// Fake Database s? d?ng static List ?? l?u tr? d? li?u
    /// Ph?c v? m?c ?ích h?c t?p, không dùng database th?t
    /// </summary>
    public static class FakeDatabase
    {
        // Collections
        public static List<User> Users { get; set; } = new List<User>();
        public static List<Student> Students { get; set; } = new List<Student>();
        public static List<Lecturer> Lecturers { get; set; } = new List<Lecturer>();
        public static List<AdministrativeClass> AdministrativeClasses { get; set; } = new List<AdministrativeClass>();
        public static List<Subject> Subjects { get; set; } = new List<Subject>();
        public static List<CourseClass> CourseClasses { get; set; } = new List<CourseClass>();
        public static List<Schedule> Schedules { get; set; } = new List<Schedule>();
        public static List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public static List<Grade> Grades { get; set; } = new List<Grade>();
        public static List<Attendance> Attendances { get; set; } = new List<Attendance>();
        public static List<Notification> Notifications { get; set; } = new List<Notification>();

        // Auto-increment IDs
        private static int _nextUserId = 1;
        private static int _nextStudentId = 1;
        private static int _nextLecturerId = 1;
        private static int _nextAdminClassId = 1;
        private static int _nextSubjectId = 1;
        private static int _nextCourseClassId = 1;
        private static int _nextScheduleId = 1;
        private static int _nextEnrollmentId = 1;
        private static int _nextGradeId = 1;
        private static int _nextAttendanceId = 1;
        private static int _nextNotificationId = 1;

        // ID Generators
        public static int GetNextUserId() => _nextUserId++;
        public static int GetNextStudentId() => _nextStudentId++;
        public static int GetNextLecturerId() => _nextLecturerId++;
        public static int GetNextAdminClassId() => _nextAdminClassId++;
        public static int GetNextSubjectId() => _nextSubjectId++;
        public static int GetNextCourseClassId() => _nextCourseClassId++;
        public static int GetNextScheduleId() => _nextScheduleId++;
        public static int GetNextEnrollmentId() => _nextEnrollmentId++;
        public static int GetNextGradeId() => _nextGradeId++;
        public static int GetNextAttendanceId() => _nextAttendanceId++;
        public static int GetNextNotificationId() => _nextNotificationId++;

        /// <summary>
        /// Kh?i t?o d? li?u seed ban ??u
        /// </summary>
        public static void Initialize()
        {
            if (Users.Any()) return; // ?ã kh?i t?o r?i

            SeedUsers();
            SeedStudents();
            SeedLecturers();
            SeedAdministrativeClasses();
            SeedSubjects();
            SeedCourseClasses();
            SeedSchedules();
            SeedEnrollments();
            SeedGrades();
            SeedAttendances();
            SeedNotifications();
        }

        private static void SeedUsers()
        {
            Users.AddRange(new[]
            {
                // Admin
                new User
                {
                    Id = GetNextUserId(),
                    Email = "admin@university.edu.vn",
                    Password = "admin123", // Plain text for demo
                    FullName = "System Administrator",
                    Role = UserRole.Admin,
                    Status = UserStatus.Active,
                    CreatedDate = DateTime.Now.AddYears(-1)
                },
                
                // Lecturers
                new User
                {
                    Id = GetNextUserId(),
                    Email = "nguyenvana@university.edu.vn",
                    Password = "lecturer123",
                    FullName = "Dr. Nguyen Van A",
                    Role = UserRole.Lecturer,
                    Status = UserStatus.Active,
                    CreatedDate = DateTime.Now.AddMonths(-6)
                },
                new User
                {
                    Id = GetNextUserId(),
                    Email = "tranthib@university.edu.vn",
                    Password = "lecturer123",
                    FullName = "Assoc. Prof. Tran Thi B",
                    Role = UserRole.Lecturer,
                    Status = UserStatus.Active,
                    CreatedDate = DateTime.Now.AddMonths(-6)
                },
                new User
                {
                    Id = GetNextUserId(),
                    Email = "levanc@university.edu.vn",
                    Password = "lecturer123",
                    FullName = "MSc. Le Van C",
                    Role = UserRole.Lecturer,
                    Status = UserStatus.Active,
                    CreatedDate = DateTime.Now.AddMonths(-5)
                },
                
                // Students
                new User
                {
                    Id = GetNextUserId(),
                    Email = "phamvand@student.edu.vn",
                    Password = "student123",
                    FullName = "Pham Van D",
                    Role = UserRole.Student,
                    Status = UserStatus.Active,
                    CreatedDate = DateTime.Now.AddMonths(-3)
                },
                new User
                {
                    Id = GetNextUserId(),
                    Email = "hoangthie@student.edu.vn",
                    Password = "student123",
                    FullName = "Hoang Thi E",
                    Role = UserRole.Student,
                    Status = UserStatus.Active,
                    CreatedDate = DateTime.Now.AddMonths(-3)
                },
                new User
                {
                    Id = GetNextUserId(),
                    Email = "vuthif@student.edu.vn",
                    Password = "student123",
                    FullName = "Vu Thi F",
                    Role = UserRole.Student,
                    Status = UserStatus.Active,
                    CreatedDate = DateTime.Now.AddMonths(-3)
                },
                new User
                {
                    Id = GetNextUserId(),
                    Email = "nguyenvang@student.edu.vn",
                    Password = "student123",
                    FullName = "Nguyen Van G",
                    Role = UserRole.Student,
                    Status = UserStatus.Active,
                    CreatedDate = DateTime.Now.AddMonths(-2)
                },
                new User
                {
                    Id = GetNextUserId(),
                    Email = "tranthih@student.edu.vn",
                    Password = "student123",
                    FullName = "Tran Thi H",
                    Role = UserRole.Student,
                    Status = UserStatus.Active,
                    CreatedDate = DateTime.Now.AddMonths(-2)
                }
            });
        }

        private static void SeedStudents()
        {
            Students.AddRange(new[]
            {
                new Student
                {
                    Id = GetNextStudentId(),
                    UserId = 5, // phamvand
                    StudentCode = "SV001",
                    FullName = "Pham Van D",
                    Email = "phamvand@student.edu.vn",
                    DateOfBirth = new DateTime(2003, 5, 15),
                    PhoneNumber = "0901234567",
                    Address = "123 Nguyen Hue, District 1, HCMC",
                    AdministrativeClassId = 1,
                    Major = "Information Technology",
                    AdmissionYear = 2021,
                    CreatedDate = DateTime.Now.AddMonths(-3)
                },
                new Student
                {
                    Id = GetNextStudentId(),
                    UserId = 6, // hoangthie
                    StudentCode = "SV002",
                    FullName = "Hoang Thi E",
                    Email = "hoangthie@student.edu.vn",
                    DateOfBirth = new DateTime(2003, 8, 20),
                    PhoneNumber = "0902345678",
                    Address = "456 Le Loi, District 1, HCMC",
                    AdministrativeClassId = 1,
                    Major = "Information Technology",
                    AdmissionYear = 2021,
                    CreatedDate = DateTime.Now.AddMonths(-3)
                },
                new Student
                {
                    Id = GetNextStudentId(),
                    UserId = 7, // vuthif
                    StudentCode = "SV003",
                    FullName = "Vu Thi F",
                    Email = "vuthif@student.edu.vn",
                    DateOfBirth = new DateTime(2003, 3, 10),
                    PhoneNumber = "0903456789",
                    Address = "789 Tran Hung Dao, District 5, HCMC",
                    AdministrativeClassId = 1,
                    Major = "Information Technology",
                    AdmissionYear = 2021,
                    CreatedDate = DateTime.Now.AddMonths(-3)
                },
                new Student
                {
                    Id = GetNextStudentId(),
                    UserId = 8, // nguyenvang
                    StudentCode = "SV004",
                    FullName = "Nguyen Van G",
                    Email = "nguyenvang@student.edu.vn",
                    DateOfBirth = new DateTime(2004, 7, 25),
                    PhoneNumber = "0904567890",
                    Address = "321 Vo Van Tan, District 3, HCMC",
                    AdministrativeClassId = 2,
                    Major = "Business Administration",
                    AdmissionYear = 2022,
                    CreatedDate = DateTime.Now.AddMonths(-2)
                },
                new Student
                {
                    Id = GetNextStudentId(),
                    UserId = 9, // tranthih
                    StudentCode = "SV005",
                    FullName = "Tran Thi H",
                    Email = "tranthih@student.edu.vn",
                    DateOfBirth = new DateTime(2004, 11, 30),
                    PhoneNumber = "0905678901",
                    Address = "654 Pasteur, District 1, HCMC",
                    AdministrativeClassId = 2,
                    Major = "Business Administration",
                    AdmissionYear = 2022,
                    CreatedDate = DateTime.Now.AddMonths(-2)
                }
            });
        }

        private static void SeedLecturers()
        {
            Lecturers.AddRange(new[]
            {
                new Lecturer
                {
                    Id = GetNextLecturerId(),
                    UserId = 2, // nguyenvana
                    LecturerCode = "GV001",
                    FullName = "Dr. Nguyen Van A",
                    Email = "nguyenvana@university.edu.vn",
                    DateOfBirth = new DateTime(1980, 3, 15),
                    PhoneNumber = "0911234567",
                    Department = "Information Technology",
                    Title = "PhD",
                    Specialization = "Artificial Intelligence",
                    JoinDate = new DateTime(2010, 9, 1)
                },
                new Lecturer
                {
                    Id = GetNextLecturerId(),
                    UserId = 3, // tranthib
                    LecturerCode = "GV002",
                    FullName = "Assoc. Prof. Tran Thi B",
                    Email = "tranthib@university.edu.vn",
                    DateOfBirth = new DateTime(1975, 7, 20),
                    PhoneNumber = "0912345678",
                    Department = "Information Technology",
                    Title = "Associate Professor",
                    Specialization = "Database",
                    JoinDate = new DateTime(2005, 9, 1)
                },
                new Lecturer
                {
                    Id = GetNextLecturerId(),
                    UserId = 4, // levanc
                    LecturerCode = "GV003",
                    FullName = "MSc. Le Van C",
                    Email = "levanc@university.edu.vn",
                    DateOfBirth = new DateTime(1985, 12, 5),
                    PhoneNumber = "0913456789",
                    Department = "Business Administration",
                    Title = "MSc",
                    Specialization = "Marketing",
                    JoinDate = new DateTime(2015, 9, 1)
                }
            });
        }

        private static void SeedAdministrativeClasses()
        {
            AdministrativeClasses.AddRange(new[]
            {
                new AdministrativeClass
                {
                    Id = GetNextAdminClassId(),
                    ClassName = "CNTT-K17A",
                    Major = "Information Technology",
                    AdmissionYear = 2021,
                    AdvisorLecturerId = 1,
                    CreatedDate = DateTime.Now.AddYears(-3)
                },
                new AdministrativeClass
                {
                    Id = GetNextAdminClassId(),
                    ClassName = "QTKD-K18A",
                    Major = "Business Administration",
                    AdmissionYear = 2022,
                    AdvisorLecturerId = 3,
                    CreatedDate = DateTime.Now.AddYears(-2)
                }
            });
        }

        private static void SeedSubjects()
        {
            Subjects.AddRange(new[]
            {
                new Subject
                {
                    Id = GetNextSubjectId(),
                    SubjectCode = "IT001",
                    SubjectName = "Introduction to Programming",
                    Credits = 4,
                    Department = "Information Technology",
                    Description = "Introduction to basic programming concepts",
                    PrerequisiteSubjectIds = new List<int>(),
                    CreatedDate = DateTime.Now.AddYears(-2)
                },
                new Subject
                {
                    Id = GetNextSubjectId(),
                    SubjectCode = "IT002",
                    SubjectName = "Data Structures & Algorithms",
                    Credits = 4,
                    Department = "Information Technology",
                    Description = "Fundamental data structures and algorithms",
                    PrerequisiteSubjectIds = new List<int> { 1 },
                    CreatedDate = DateTime.Now.AddYears(-2)
                },
                new Subject
                {
                    Id = GetNextSubjectId(),
                    SubjectCode = "IT003",
                    SubjectName = "Database Systems",
                    Credits = 4,
                    Department = "Information Technology",
                    Description = "Design and management of database systems",
                    PrerequisiteSubjectIds = new List<int> { 1 },
                    CreatedDate = DateTime.Now.AddYears(-2)
                },
                new Subject
                {
                    Id = GetNextSubjectId(),
                    SubjectCode = "IT004",
                    SubjectName = "Web Programming",
                    Credits = 4,
                    Department = "Information Technology",
                    Description = "Developing web applications with ASP.NET Core",
                    PrerequisiteSubjectIds = new List<int> { 1, 3 },
                    CreatedDate = DateTime.Now.AddYears(-2)
                },
                new Subject
                {
                    Id = GetNextSubjectId(),
                    SubjectCode = "BA001",
                    SubjectName = "Principles of Management",
                    Credits = 3,
                    Department = "Business Administration",
                    Description = "Basic principles of management",
                    PrerequisiteSubjectIds = new List<int>(),
                    CreatedDate = DateTime.Now.AddYears(-2)
                },
                new Subject
                {
                    Id = GetNextSubjectId(),
                    SubjectCode = "BA002",
                    SubjectName = "Introduction to Marketing",
                    Credits = 3,
                    Department = "Business Administration",
                    Description = "Fundamental marketing concepts and strategies",
                    PrerequisiteSubjectIds = new List<int> { 5 },
                    CreatedDate = DateTime.Now.AddYears(-2)
                }
            });
        }

        private static void SeedCourseClasses()
        {
            CourseClasses.AddRange(new[]
            {
                new CourseClass
                {
                    Id = GetNextCourseClassId(),
                    ClassCode = "IT001.01",
                    SubjectId = 1,
                    LecturerId = 1,
                    Semester = "HK1-2024",
                    MaxStudents = 50,
                    CurrentStudents = 3,
                    Room = "A101",
                    Status = CourseClassStatus.InProgress,
                    CreatedDate = DateTime.Now.AddMonths(-2)
                },
                new CourseClass
                {
                    Id = GetNextCourseClassId(),
                    ClassCode = "IT002.01",
                    SubjectId = 2,
                    LecturerId = 1,
                    Semester = "HK1-2024",
                    MaxStudents = 50,
                    CurrentStudents = 2,
                    Room = "A102",
                    Status = CourseClassStatus.InProgress,
                    CreatedDate = DateTime.Now.AddMonths(-2)
                },
                new CourseClass
                {
                    Id = GetNextCourseClassId(),
                    ClassCode = "IT003.01",
                    SubjectId = 3,
                    LecturerId = 2,
                    Semester = "HK1-2024",
                    MaxStudents = 45,
                    CurrentStudents = 1,
                    Room = "B201",
                    Status = CourseClassStatus.InProgress,
                    CreatedDate = DateTime.Now.AddMonths(-2)
                },
                new CourseClass
                {
                    Id = GetNextCourseClassId(),
                    ClassCode = "IT004.01",
                    SubjectId = 4,
                    LecturerId = 2,
                    Semester = "HK1-2024",
                    MaxStudents = 40,
                    CurrentStudents = 0,
                    Room = "Lab301",
                    Status = CourseClassStatus.Open,
                    CreatedDate = DateTime.Now.AddMonths(-1)
                },
                new CourseClass
                {
                    Id = GetNextCourseClassId(),
                    ClassCode = "BA001.01",
                    SubjectId = 5,
                    LecturerId = 3,
                    Semester = "HK1-2024",
                    MaxStudents = 60,
                    CurrentStudents = 2,
                    Room = "C301",
                    Status = CourseClassStatus.InProgress,
                    CreatedDate = DateTime.Now.AddMonths(-2)
                },
                new CourseClass
                {
                    Id = GetNextCourseClassId(),
                    ClassCode = "BA002.01",
                    SubjectId = 6,
                    LecturerId = 3,
                    Semester = "HK1-2024",
                    MaxStudents = 60,
                    CurrentStudents = 0,
                    Room = "C302",
                    Status = CourseClassStatus.Open,
                    CreatedDate = DateTime.Now.AddMonths(-1)
                }
            });
        }

        private static void SeedSchedules()
        {
            Schedules.AddRange(new[]
            {
                // IT001.01
                new Schedule
                {
                    Id = GetNextScheduleId(),
                    CourseClassId = 1,
                    DayOfWeek = DayOfWeek.Monday,
                    Session = "Morning",
                    Period = "Period 1-3",
                    StartTime = "07:00",
                    EndTime = "09:30",
                    Room = "A101",
                    EffectiveDate = DateTime.Now.AddMonths(-2),
                    CreatedDate = DateTime.Now.AddMonths(-2)
                },
                new Schedule
                {
                    Id = GetNextScheduleId(),
                    CourseClassId = 1,
                    DayOfWeek = DayOfWeek.Wednesday,
                    Session = "Morning",
                    Period = "Period 1-3",
                    StartTime = "07:00",
                    EndTime = "09:30",
                    Room = "A101",
                    EffectiveDate = DateTime.Now.AddMonths(-2),
                    CreatedDate = DateTime.Now.AddMonths(-2)
                },
                
                // IT002.01
                new Schedule
                {
                    Id = GetNextScheduleId(),
                    CourseClassId = 2,
                    DayOfWeek = DayOfWeek.Tuesday,
                    Session = "Afternoon",
                    Period = "Period 4-6",
                    StartTime = "13:00",
                    EndTime = "15:30",
                    Room = "A102",
                    EffectiveDate = DateTime.Now.AddMonths(-2),
                    CreatedDate = DateTime.Now.AddMonths(-2)
                },
                new Schedule
                {
                    Id = GetNextScheduleId(),
                    CourseClassId = 2,
                    DayOfWeek = DayOfWeek.Thursday,
                    Session = "Afternoon",
                    Period = "Period 4-6",
                    StartTime = "13:00",
                    EndTime = "15:30",
                    Room = "A102",
                    EffectiveDate = DateTime.Now.AddMonths(-2),
                    CreatedDate = DateTime.Now.AddMonths(-2)
                },
                
                // IT003.01
                new Schedule
                {
                    Id = GetNextScheduleId(),
                    CourseClassId = 3,
                    DayOfWeek = DayOfWeek.Friday,
                    Session = "Morning",
                    Period = "Period 1-3",
                    StartTime = "07:00",
                    EndTime = "09:30",
                    Room = "B201",
                    EffectiveDate = DateTime.Now.AddMonths(-2),
                    CreatedDate = DateTime.Now.AddMonths(-2)
                },
                
                // BA001.01
                new Schedule
                {
                    Id = GetNextScheduleId(),
                    CourseClassId = 5,
                    DayOfWeek = DayOfWeek.Monday,
                    Session = "Afternoon",
                    Period = "Period 4-6",
                    StartTime = "13:00",
                    EndTime = "15:30",
                    Room = "C301",
                    EffectiveDate = DateTime.Now.AddMonths(-2),
                    CreatedDate = DateTime.Now.AddMonths(-2)
                }
            });
        }

        private static void SeedEnrollments()
        {
            Enrollments.AddRange(new[]
            {
                // Student 1 (SV001) enrollments
                new Enrollment
                {
                    Id = GetNextEnrollmentId(),
                    StudentId = 1,
                    CourseClassId = 1, // IT001.01
                    EnrollmentDate = DateTime.Now.AddMonths(-2),
                    Status = EnrollmentStatus.Approved,
                    ApprovedDate = DateTime.Now.AddMonths(-2),
                    ApprovedBy = 1
                },
                new Enrollment
                {
                    Id = GetNextEnrollmentId(),
                    StudentId = 1,
                    CourseClassId = 2, // IT002.01
                    EnrollmentDate = DateTime.Now.AddMonths(-2),
                    Status = EnrollmentStatus.Approved,
                    ApprovedDate = DateTime.Now.AddMonths(-2),
                    ApprovedBy = 1
                },
                
                // Student 2 (SV002) enrollments
                new Enrollment
                {
                    Id = GetNextEnrollmentId(),
                    StudentId = 2,
                    CourseClassId = 1, // IT001.01
                    EnrollmentDate = DateTime.Now.AddMonths(-2),
                    Status = EnrollmentStatus.Approved,
                    ApprovedDate = DateTime.Now.AddMonths(-2),
                    ApprovedBy = 1
                },
                new Enrollment
                {
                    Id = GetNextEnrollmentId(),
                    StudentId = 2,
                    CourseClassId = 3, // IT003.01
                    EnrollmentDate = DateTime.Now.AddMonths(-2),
                    Status = EnrollmentStatus.Approved,
                    ApprovedDate = DateTime.Now.AddMonths(-2),
                    ApprovedBy = 1
                },
                
                // Student 3 (SV003) enrollments
                new Enrollment
                {
                    Id = GetNextEnrollmentId(),
                    StudentId = 3,
                    CourseClassId = 1, // IT001.01
                    EnrollmentDate = DateTime.Now.AddMonths(-2),
                    Status = EnrollmentStatus.Approved,
                    ApprovedDate = DateTime.Now.AddMonths(-2),
                    ApprovedBy = 1
                },
                new Enrollment
                {
                    Id = GetNextEnrollmentId(),
                    StudentId = 3,
                    CourseClassId = 2, // IT002.01
                    EnrollmentDate = DateTime.Now.AddMonths(-1),
                    Status = EnrollmentStatus.Pending
                },
                
                // Student 4 (SV004) enrollments
                new Enrollment
                {
                    Id = GetNextEnrollmentId(),
                    StudentId = 4,
                    CourseClassId = 5, // BA001.01
                    EnrollmentDate = DateTime.Now.AddMonths(-2),
                    Status = EnrollmentStatus.Approved,
                    ApprovedDate = DateTime.Now.AddMonths(-2),
                    ApprovedBy = 1
                },
                
                // Student 5 (SV005) enrollments
                new Enrollment
                {
                    Id = GetNextEnrollmentId(),
                    StudentId = 5,
                    CourseClassId = 5, // BA001.01
                    EnrollmentDate = DateTime.Now.AddMonths(-2),
                    Status = EnrollmentStatus.Approved,
                    ApprovedDate = DateTime.Now.AddMonths(-2),
                    ApprovedBy = 1
                }
            });
        }

        private static void SeedGrades()
        {
            Grades.AddRange(new[]
            {
                // Student 1 - IT001.01
                new Grade
                {
                    Id = GetNextGradeId(),
                    EnrollmentId = 1,
                    StudentId = 1,
                    CourseClassId = 1,
                    AttendanceScore = 9.0,
                    MidtermScore = 8.5,
                    FinalScore = 8.0,
                    TotalScore = 8.3,
                    LetterGrade = "B+",
                    IsPassed = true,
                    LastUpdated = DateTime.Now.AddDays(-10),
                    UpdatedBy = 1
                },
                
                // Student 1 - IT002.01
                new Grade
                {
                    Id = GetNextGradeId(),
                    EnrollmentId = 2,
                    StudentId = 1,
                    CourseClassId = 2,
                    AttendanceScore = 8.5,
                    MidtermScore = 7.5,
                    FinalScore = null, // Ch?a có ?i?m cu?i k?
                    TotalScore = null,
                    LetterGrade = null,
                    IsPassed = false,
                    LastUpdated = DateTime.Now.AddDays(-5),
                    UpdatedBy = 1
                },
                
                // Student 2 - IT001.01
                new Grade
                {
                    Id = GetNextGradeId(),
                    EnrollmentId = 3,
                    StudentId = 2,
                    CourseClassId = 1,
                    AttendanceScore = 10.0,
                    MidtermScore = 9.0,
                    FinalScore = 9.5,
                    TotalScore = 9.4,
                    LetterGrade = "A",
                    IsPassed = true,
                    LastUpdated = DateTime.Now.AddDays(-10),
                    UpdatedBy = 1
                },
                
                // Student 4 - BA001.01
                new Grade
                {
                    Id = GetNextGradeId(),
                    EnrollmentId = 7,
                    StudentId = 4,
                    CourseClassId = 5,
                    AttendanceScore = 7.0,
                    MidtermScore = null, // Ch?a có ?i?m gi?a k?
                    FinalScore = null,
                    TotalScore = null,
                    LetterGrade = null,
                    IsPassed = false,
                    LastUpdated = DateTime.Now.AddDays(-3),
                    UpdatedBy = 3
                }
            });
        }

        private static void SeedAttendances()
        {
            // T?o m?t s? b?n ghi ?i?m danh m?u
            var today = DateTime.Today;
            
            Attendances.AddRange(new[]
            {
                // Student 1 - IT001.01
                new Attendance
                {
                    Id = GetNextAttendanceId(),
                    EnrollmentId = 1,
                    StudentId = 1,
                    CourseClassId = 1,
                    AttendanceDate = today.AddDays(-7),
                    Session = "Period 1-3",
                    Status = AttendanceStatus.Present,
                    CreatedDate = today.AddDays(-7),
                    CreatedBy = 1
                },
                new Attendance
                {
                    Id = GetNextAttendanceId(),
                    EnrollmentId = 1,
                    StudentId = 1,
                    CourseClassId = 1,
                    AttendanceDate = today.AddDays(-5),
                    Session = "Period 1-3",
                    Status = AttendanceStatus.Present,
                    CreatedDate = today.AddDays(-5),
                    CreatedBy = 1
                },
                
                // Student 2 - IT001.01
                new Attendance
                {
                    Id = GetNextAttendanceId(),
                    EnrollmentId = 3,
                    StudentId = 2,
                    CourseClassId = 1,
                    AttendanceDate = today.AddDays(-7),
                    Session = "Period 1-3",
                    Status = AttendanceStatus.Present,
                    CreatedDate = today.AddDays(-7),
                    CreatedBy = 1
                },
                new Attendance
                {
                    Id = GetNextAttendanceId(),
                    EnrollmentId = 3,
                    StudentId = 2,
                    CourseClassId = 1,
                    AttendanceDate = today.AddDays(-5),
                    Session = "Period 1-3",
                    Status = AttendanceStatus.Late,
                    Note = "Late by 10 minutes",
                    CreatedDate = today.AddDays(-5),
                    CreatedBy = 1
                }
            });
        }

        private static void SeedNotifications()
        {
            Notifications.AddRange(new[]
            {
                // Notification cho Student 1
                new Notification
                {
                    Id = GetNextNotificationId(),
                    UserId = 5,
                    Title = "Course registration successful",
                    Message = "You have successfully registered for IT001 - Introduction to Programming",
                    Type = NotificationType.Enrollment,
                    IsRead = true,
                    LinkUrl = "/Student/Enrollments",
                    CreatedDate = DateTime.Now.AddMonths(-2),
                    ReadDate = DateTime.Now.AddMonths(-2).AddHours(2)
                },
                new Notification
                {
                    Id = GetNextNotificationId(),
                    UserId = 5,
                    Title = "New grade available",
                    Message = "Grades for IT001 have been updated. View details on the Grades page.",
                    Type = NotificationType.Grade,
                    IsRead = false,
                    LinkUrl = "/Student/Grades",
                    CreatedDate = DateTime.Now.AddDays(-10)
                },
                
                // Notification cho Student 2
                new Notification
                {
                    Id = GetNextNotificationId(),
                    UserId = 6,
                    Title = "Schedule update",
                    Message = "Schedule for IT001 has been updated. Please check your timetable.",
                    Type = NotificationType.Schedule,
                    IsRead = false,
                    LinkUrl = "/Student/Schedule",
                    CreatedDate = DateTime.Now.AddDays(-3)
                },
                
                // Notification cho Student 3
                new Notification
                {
                    Id = GetNextNotificationId(),
                    UserId = 7,
                    Title = "Registration pending",
                    Message = "Your registration for IT002 is pending review.",
                    Type = NotificationType.Enrollment,
                    IsRead = false,
                    LinkUrl = "/Student/Enrollments",
                    CreatedDate = DateTime.Now.AddDays(-1)
                },
                
                // Notification cho Lecturer
                new Notification
                {
                    Id = GetNextNotificationId(),
                    UserId = 2,
                    Title = "System Notice",
                    Message = "Please update final grades before 30/12/2024.",
                    Type = NotificationType.System,
                    IsRead = false,
                    CreatedDate = DateTime.Now.AddDays(-2)
                }
            });
        }
    }
}
