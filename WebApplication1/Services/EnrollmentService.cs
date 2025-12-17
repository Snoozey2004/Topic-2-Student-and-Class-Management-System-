using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service qu?n lý Enrollment
    /// </summary>
    public interface IEnrollmentService
    {
        List<EnrollmentListViewModel> GetAll(EnrollmentStatus? status = null);
        List<EnrollmentListViewModel> GetByStudentId(int studentId);
        List<EnrollmentListViewModel> GetByCourseClassId(int courseClassId);
        Enrollment? GetById(int id);
        bool Enroll(int studentId, int courseClassId);
        bool Approve(int enrollmentId, int approvedBy);
        bool Reject(int enrollmentId, string reason);
        bool Drop(int enrollmentId);
        List<AvailableCourseViewModel> GetAvailableCoursesForStudent(int studentId);
        bool CanEnroll(int studentId, int courseClassId, out string message);
    }

    public class EnrollmentService : IEnrollmentService
    {
        private readonly INotificationService _notificationService;

        public EnrollmentService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public List<EnrollmentListViewModel> GetAll(EnrollmentStatus? status = null)
        {
            var query = FakeDatabase.Enrollments.AsEnumerable();

            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status.Value);
            }

            return query.Select(e =>
            {
                var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == e.StudentId);
                var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == e.CourseClassId);
                var subject = courseClass != null
                    ? FakeDatabase.Subjects.FirstOrDefault(s => s.Id == courseClass.SubjectId)
                    : null;

                return new EnrollmentListViewModel
                {
                    Id = e.Id,
                    StudentCode = student?.StudentCode ?? "",
                    StudentName = student?.FullName ?? "",
                    ClassCode = courseClass?.ClassCode ?? "",
                    SubjectName = subject?.SubjectName ?? "",
                    Semester = courseClass?.Semester ?? "",
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status.ToString(),
                    RejectionReason = e.RejectionReason
                };
            }).OrderByDescending(e => e.EnrollmentDate).ToList();
        }

        public List<EnrollmentListViewModel> GetByStudentId(int studentId)
        {
            return FakeDatabase.Enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e =>
                {
                    var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == e.StudentId);
                    var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == e.CourseClassId);
                    var subject = courseClass != null
                        ? FakeDatabase.Subjects.FirstOrDefault(s => s.Id == courseClass.SubjectId)
                        : null;

                    return new EnrollmentListViewModel
                    {
                        Id = e.Id,
                        StudentCode = student?.StudentCode ?? "",
                        StudentName = student?.FullName ?? "",
                        ClassCode = courseClass?.ClassCode ?? "",
                        SubjectName = subject?.SubjectName ?? "",
                        Semester = courseClass?.Semester ?? "",
                        EnrollmentDate = e.EnrollmentDate,
                        Status = e.Status.ToString(),
                        RejectionReason = e.RejectionReason
                    };
                }).OrderByDescending(e => e.EnrollmentDate).ToList();
        }

        public List<EnrollmentListViewModel> GetByCourseClassId(int courseClassId)
        {
            return FakeDatabase.Enrollments
                .Where(e => e.CourseClassId == courseClassId)
                .Select(e =>
                {
                    var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == e.StudentId);
                    var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == e.CourseClassId);
                    var subject = courseClass != null
                        ? FakeDatabase.Subjects.FirstOrDefault(s => s.Id == courseClass.SubjectId)
                        : null;

                    return new EnrollmentListViewModel
                    {
                        Id = e.Id,
                        StudentCode = student?.StudentCode ?? "",
                        StudentName = student?.FullName ?? "",
                        ClassCode = courseClass?.ClassCode ?? "",
                        SubjectName = subject?.SubjectName ?? "",
                        Semester = courseClass?.Semester ?? "",
                        EnrollmentDate = e.EnrollmentDate,
                        Status = e.Status.ToString(),
                        RejectionReason = e.RejectionReason
                    };
                }).OrderBy(e => e.StudentCode).ToList();
        }

        public Enrollment? GetById(int id)
        {
            return FakeDatabase.Enrollments.FirstOrDefault(e => e.Id == id);
        }

        public bool Enroll(int studentId, int courseClassId)
        {
            // Ki?m tra ?i?u ki?n ??ng ký
            if (!CanEnroll(studentId, courseClassId, out string message))
            {
                return false;
            }

            var enrollment = new Enrollment
            {
                Id = FakeDatabase.GetNextEnrollmentId(),
                StudentId = studentId,
                CourseClassId = courseClassId,
                EnrollmentDate = DateTime.Now,
                Status = EnrollmentStatus.Pending
            };

            FakeDatabase.Enrollments.Add(enrollment);

            // T?o thông báo cho sinh viên
            var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == studentId);
            if (student != null)
            {
                _notificationService.CreateNotification(
                    student.UserId,
                    "??ng ký môn h?c",
                    "??n ??ng ký môn h?c c?a b?n ?ang ch? ???c duy?t.",
                    NotificationType.Enrollment,
                    "/Student/Enrollments"
                );
            }

            return true;
        }

        public bool Approve(int enrollmentId, int approvedBy)
        {
            var enrollment = GetById(enrollmentId);
            if (enrollment == null || enrollment.Status != EnrollmentStatus.Pending)
            {
                return false;
            }

            enrollment.Status = EnrollmentStatus.Approved;
            enrollment.ApprovedDate = DateTime.Now;
            enrollment.ApprovedBy = approvedBy;

            // C?p nh?t s? l??ng sinh viên trong l?p
            var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == enrollment.CourseClassId);
            if (courseClass != null)
            {
                courseClass.CurrentStudents++;
            }

            // T?o b?n ghi Grade
            var grade = new Grade
            {
                Id = FakeDatabase.GetNextGradeId(),
                EnrollmentId = enrollment.Id,
                StudentId = enrollment.StudentId,
                CourseClassId = enrollment.CourseClassId,
                IsPassed = false
            };
            FakeDatabase.Grades.Add(grade);

            // T?o thông báo cho sinh viên
            var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == enrollment.StudentId);
            if (student != null)
            {
                _notificationService.CreateNotification(
                    student.UserId,
                    "??ng ký môn h?c thành công",
                    "??n ??ng ký môn h?c c?a b?n ?ã ???c phê duy?t.",
                    NotificationType.Enrollment,
                    "/Student/Enrollments"
                );
            }

            return true;
        }

        public bool Reject(int enrollmentId, string reason)
        {
            var enrollment = GetById(enrollmentId);
            if (enrollment == null || enrollment.Status != EnrollmentStatus.Pending)
            {
                return false;
            }

            enrollment.Status = EnrollmentStatus.Rejected;
            enrollment.RejectionReason = reason;

            // T?o thông báo cho sinh viên
            var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == enrollment.StudentId);
            if (student != null)
            {
                _notificationService.CreateNotification(
                    student.UserId,
                    "??ng ký môn h?c b? t? ch?i",
                    $"??n ??ng ký môn h?c c?a b?n ?ã b? t? ch?i. Lý do: {reason}",
                    NotificationType.Enrollment,
                    "/Student/Enrollments"
                );
            }

            return true;
        }

        public bool Drop(int enrollmentId)
        {
            var enrollment = GetById(enrollmentId);
            if (enrollment == null) return false;

            enrollment.Status = EnrollmentStatus.Dropped;

            // Gi?m s? l??ng sinh viên n?u ?ã approved
            if (enrollment.Status == EnrollmentStatus.Approved)
            {
                var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == enrollment.CourseClassId);
                if (courseClass != null && courseClass.CurrentStudents > 0)
                {
                    courseClass.CurrentStudents--;
                }
            }

            return true;
        }

        public List<AvailableCourseViewModel> GetAvailableCoursesForStudent(int studentId)
        {
            var currentSemester = "HK1-2024"; // Có th? l?y t? config ho?c tính toán

            return FakeDatabase.CourseClasses
                .Where(c => c.Semester == currentSemester && c.Status == CourseClassStatus.Open)
                .Select(c =>
                {
                    var subject = FakeDatabase.Subjects.FirstOrDefault(s => s.Id == c.SubjectId);
                    var lecturer = FakeDatabase.Lecturers.FirstOrDefault(l => l.Id == c.LecturerId);
                    var schedules = FakeDatabase.Schedules
                        .Where(s => s.CourseClassId == c.Id)
                        .Select(s => $"{s.DayOfWeek}: {s.StartTime}-{s.EndTime} ({s.Room})")
                        .ToList();

                    var canEnroll = CanEnroll(studentId, c.Id, out string message);

                    return new AvailableCourseViewModel
                    {
                        CourseClassId = c.Id,
                        ClassCode = c.ClassCode,
                        SubjectCode = subject?.SubjectCode ?? "",
                        SubjectName = subject?.SubjectName ?? "",
                        Credits = subject?.Credits ?? 0,
                        LecturerName = lecturer?.FullName ?? "",
                        Semester = c.Semester,
                        CurrentStudents = c.CurrentStudents,
                        MaxStudents = c.MaxStudents,
                        Room = c.Room,
                        ScheduleInfo = schedules,
                        CanEnroll = canEnroll,
                        EnrollmentMessage = message
                    };
                }).ToList();
        }

        public bool CanEnroll(int studentId, int courseClassId, out string message)
        {
            // Ki?m tra ?ã ??ng ký r?i ch?a
            var existingEnrollment = FakeDatabase.Enrollments.FirstOrDefault(e =>
                e.StudentId == studentId &&
                e.CourseClassId == courseClassId &&
                e.Status != EnrollmentStatus.Rejected &&
                e.Status != EnrollmentStatus.Dropped);

            if (existingEnrollment != null)
            {
                message = "B?n ?ã ??ng ký l?p này r?i";
                return false;
            }

            // Ki?m tra l?p ?ã ??y ch?a
            var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == courseClassId);
            if (courseClass == null)
            {
                message = "L?p h?c không t?n t?i";
                return false;
            }

            if (courseClass.CurrentStudents >= courseClass.MaxStudents)
            {
                message = "L?p h?c ?ã ??y";
                return false;
            }

            if (courseClass.Status != CourseClassStatus.Open)
            {
                message = "L?p h?c không m? ??ng ký";
                return false;
            }

            // Ki?m tra môn tiên quy?t
            var subject = FakeDatabase.Subjects.FirstOrDefault(s => s.Id == courseClass.SubjectId);
            if (subject != null && subject.PrerequisiteSubjectIds.Any())
            {
                var passedSubjects = FakeDatabase.Enrollments
                    .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved)
                    .Join(FakeDatabase.Grades,
                        e => e.Id,
                        g => g.EnrollmentId,
                        (e, g) => new { e, g })
                    .Where(eg => eg.g.IsPassed)
                    .Join(FakeDatabase.CourseClasses,
                        eg => eg.e.CourseClassId,
                        c => c.Id,
                        (eg, c) => c.SubjectId)
                    .ToList();

                var missingPrerequisites = subject.PrerequisiteSubjectIds
                    .Where(pid => !passedSubjects.Contains(pid))
                    .ToList();

                if (missingPrerequisites.Any())
                {
                    message = "Ch?a ?? ?i?u ki?n môn tiên quy?t";
                    return false;
                }
            }

            message = "Có th? ??ng ký";
            return true;
        }
    }
}
