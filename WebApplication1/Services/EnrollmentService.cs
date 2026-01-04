using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service quản lý Enrollment (SQL Server + EF Core)
    /// </summary>
    public interface IEnrollmentService
    {
        List<EnrollmentListViewModel> GetAll(EnrollmentStatus? status = null);
        List<EnrollmentListViewModel> GetByStudentId(int studentId);
        List<EnrollmentListViewModel> GetByCourseClassId(int courseClassId);
        Enrollment? GetById(int id);
        bool Create(EnrollmentFormViewModel model);
        bool Update(EnrollmentFormViewModel model);
        bool Delete(int id);
        bool Enroll(int studentId, int courseClassId, bool autoApprove = true);
        bool Approve(int enrollmentId, int approvedBy);
        bool Reject(int enrollmentId, string reason);
        bool Drop(int enrollmentId);
        List<AvailableCourseViewModel> GetAvailableCoursesForStudent(int studentId, string? semester = null);
        bool CanEnroll(int studentId, int courseClassId, out string message);
    }

    public class EnrollmentService : IEnrollmentService
    {
        private readonly ApplicationDbContext _db;
        private readonly INotificationService _notificationService;

        public EnrollmentService(ApplicationDbContext db, INotificationService notificationService)
        {
            _db = db;
            _notificationService = notificationService;
        }

        public List<EnrollmentListViewModel> GetAll(EnrollmentStatus? status = null)
        {
            var query =
                from e in _db.Enrollments.AsNoTracking()
                join st in _db.Students.AsNoTracking() on e.StudentId equals st.Id
                join cc in _db.CourseClasses.AsNoTracking() on e.CourseClassId equals cc.Id
                join sub in _db.Subjects.AsNoTracking() on cc.SubjectId equals sub.Id
                select new { e, st, cc, sub };

            if (status.HasValue)
                query = query.Where(x => x.e.Status == status.Value);

            return query
                .OrderByDescending(x => x.e.EnrollmentDate)
                .Select(x => new EnrollmentListViewModel
                {
                    Id = x.e.Id,
                    StudentCode = x.st.StudentCode,
                    StudentName = x.st.FullName,
                    ClassCode = x.cc.ClassCode,
                    SubjectName = x.sub.SubjectName,
                    Semester = x.cc.Semester,
                    EnrollmentDate = x.e.EnrollmentDate,
                    Status = x.e.Status.ToString(),
                    RejectionReason = x.e.RejectionReason
                })
                .ToList();
        }

        public List<EnrollmentListViewModel> GetByStudentId(int studentId)
        {
            var query =
                from e in _db.Enrollments.AsNoTracking()
                join st in _db.Students.AsNoTracking() on e.StudentId equals st.Id
                join cc in _db.CourseClasses.AsNoTracking() on e.CourseClassId equals cc.Id
                join sub in _db.Subjects.AsNoTracking() on cc.SubjectId equals sub.Id
                where e.StudentId == studentId
                      && e.Status != EnrollmentStatus.Dropped
                      && e.Status != EnrollmentStatus.Rejected
                orderby e.EnrollmentDate descending
                select new EnrollmentListViewModel
                {
                    Id = e.Id,
                    StudentCode = st.StudentCode,
                    StudentName = st.FullName,
                    ClassCode = cc.ClassCode,
                    SubjectName = sub.SubjectName,
                    Semester = cc.Semester,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status.ToString(),
                    RejectionReason = e.RejectionReason
                };

            return query.ToList();
        }

        public List<EnrollmentListViewModel> GetByCourseClassId(int courseClassId)
        {
            var query =
                from e in _db.Enrollments.AsNoTracking()
                join st in _db.Students.AsNoTracking() on e.StudentId equals st.Id
                join cc in _db.CourseClasses.AsNoTracking() on e.CourseClassId equals cc.Id
                join sub in _db.Subjects.AsNoTracking() on cc.SubjectId equals sub.Id
                where e.CourseClassId == courseClassId
                orderby st.StudentCode
                select new EnrollmentListViewModel
                {
                    Id = e.Id,
                    StudentCode = st.StudentCode,
                    StudentName = st.FullName,
                    ClassCode = cc.ClassCode,
                    SubjectName = sub.SubjectName,
                    Semester = cc.Semester,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status.ToString(),
                    RejectionReason = e.RejectionReason
                };

            return query.ToList();
        }

        public Enrollment? GetById(int id)
        {
            return _db.Enrollments.FirstOrDefault(e => e.Id == id);
        }

        public bool Create(EnrollmentFormViewModel model)
        {
            var existing = _db.Enrollments.FirstOrDefault(e =>
                e.StudentId == model.StudentId &&
                e.CourseClassId == model.CourseClassId &&
                e.Status != EnrollmentStatus.Rejected &&
                e.Status != EnrollmentStatus.Dropped);

            if (existing != null) return false;

            var enrollment = new Enrollment
            {
                StudentId = model.StudentId,
                CourseClassId = model.CourseClassId,
                EnrollmentDate = DateTime.Now,
                Status = EnrollmentStatus.Pending
            };

            _db.Enrollments.Add(enrollment);
            _db.SaveChanges();
            return true;
        }

        public bool Update(EnrollmentFormViewModel model)
        {
            var enrollment = GetById(model.Id ?? 0);
            if (enrollment == null) return false;

            enrollment.StudentId = model.StudentId;
            enrollment.CourseClassId = model.CourseClassId;
            _db.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var enrollment = _db.Enrollments.FirstOrDefault(e => e.Id == id);
            if (enrollment == null) return false;

            // nếu approved -> giảm CurrentStudents và xóa Grade
            if (enrollment.Status == EnrollmentStatus.Approved)
            {
                var courseClass = _db.CourseClasses.FirstOrDefault(c => c.Id == enrollment.CourseClassId);
                if (courseClass != null && courseClass.CurrentStudents > 0)
                    courseClass.CurrentStudents--;

                var grade = _db.Grades.FirstOrDefault(g => g.EnrollmentId == enrollment.Id);
                if (grade != null) _db.Grades.Remove(grade);
            }

            _db.Enrollments.Remove(enrollment);
            _db.SaveChanges();
            return true;
        }

        public bool Enroll(int studentId, int courseClassId, bool autoApprove = true)
        {
            if (!CanEnroll(studentId, courseClassId, out _))
                return false;

            var courseClass = _db.CourseClasses.FirstOrDefault(c => c.Id == courseClassId);
            if (courseClass == null) return false;

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseClassId = courseClassId,
                EnrollmentDate = DateTime.Now,
                Status = autoApprove ? EnrollmentStatus.Approved : EnrollmentStatus.Pending
            };

            if (autoApprove)
            {
                enrollment.ApprovedDate = DateTime.Now;
                enrollment.ApprovedBy = 0; // system

                courseClass.CurrentStudents++;
            }

            _db.Enrollments.Add(enrollment);
            _db.SaveChanges(); // để lấy enrollment.Id

            if (autoApprove)
            {
                // tạo Grade sau khi enrollment đã có Id
                var grade = new Grade
                {
                    EnrollmentId = enrollment.Id,
                    StudentId = studentId,
                    CourseClassId = courseClassId,
                    IsPassed = false
                };
                _db.Grades.Add(grade);
                _db.SaveChanges();
            }

            // notify student
            var student = _db.Students.AsNoTracking().FirstOrDefault(s => s.Id == studentId);
            if (student != null)
            {
                if (autoApprove)
                {
                    _notificationService.CreateNotification(
                        student.UserId,
                        "Course Registration Successful",
                        $"You have been enrolled in {courseClass.ClassCode}. Check your schedule for class times.",
                        NotificationType.Enrollment,
                        "/Student/Schedule"
                    );
                }
                else
                {
                    _notificationService.CreateNotification(
                        student.UserId,
                        "Course Registration Pending",
                        "Your course registration is pending approval.",
                        NotificationType.Enrollment,
                        "/Student/Enrollments"
                    );
                }
            }

            return true;
        }

        public bool Approve(int enrollmentId, int approvedBy)
        {
            var enrollment = _db.Enrollments.FirstOrDefault(e => e.Id == enrollmentId);
            if (enrollment == null || enrollment.Status != EnrollmentStatus.Pending)
                return false;

            enrollment.Status = EnrollmentStatus.Approved;
            enrollment.ApprovedDate = DateTime.Now;
            enrollment.ApprovedBy = approvedBy;

            var courseClass = _db.CourseClasses.FirstOrDefault(c => c.Id == enrollment.CourseClassId);
            if (courseClass != null) courseClass.CurrentStudents++;

            // đảm bảo chỉ có 1 grade/enrollment
            var gradeExists = _db.Grades.Any(g => g.EnrollmentId == enrollment.Id);
            if (!gradeExists)
            {
                _db.Grades.Add(new Grade
                {
                    EnrollmentId = enrollment.Id,
                    StudentId = enrollment.StudentId,
                    CourseClassId = enrollment.CourseClassId,
                    IsPassed = false
                });
            }

            _db.SaveChanges();

            var student = _db.Students.AsNoTracking().FirstOrDefault(s => s.Id == enrollment.StudentId);
            if (student != null)
            {
                _notificationService.CreateNotification(
                    student.UserId,
                    "Course Registration Approved",
                    $"Your registration for {courseClass?.ClassCode} has been approved. Check your schedule.",
                    NotificationType.Enrollment,
                    "/Student/Schedule"
                );
            }

            return true;
        }

        public bool Reject(int enrollmentId, string reason)
        {
            var enrollment = _db.Enrollments.FirstOrDefault(e => e.Id == enrollmentId);
            if (enrollment == null || enrollment.Status != EnrollmentStatus.Pending)
                return false;

            enrollment.Status = EnrollmentStatus.Rejected;
            enrollment.RejectionReason = reason;
            _db.SaveChanges();

            var student = _db.Students.AsNoTracking().FirstOrDefault(s => s.Id == enrollment.StudentId);
            if (student != null)
            {
                _notificationService.CreateNotification(
                    student.UserId,
                    "Course Registration Rejected",
                    $"Your course registration has been rejected. Reason: {reason}",
                    NotificationType.Enrollment,
                    "/Student/Enrollments"
                );
            }

            return true;
        }

        public bool Drop(int enrollmentId)
        {
            var enrollment = _db.Enrollments.FirstOrDefault(e => e.Id == enrollmentId);
            if (enrollment == null) return false;

            var wasApproved = enrollment.Status == EnrollmentStatus.Approved;
            enrollment.Status = EnrollmentStatus.Dropped;

            if (wasApproved)
            {
                var courseClass = _db.CourseClasses.FirstOrDefault(c => c.Id == enrollment.CourseClassId);
                if (courseClass != null && courseClass.CurrentStudents > 0)
                    courseClass.CurrentStudents--;

                var grade = _db.Grades.FirstOrDefault(g => g.EnrollmentId == enrollmentId);
                if (grade != null) _db.Grades.Remove(grade);
            }

            _db.SaveChanges();
            return true;
        }

        public List<AvailableCourseViewModel> GetAvailableCoursesForStudent(int studentId, string? semester = null)
        {
            var currentSemester = semester;

            if (string.IsNullOrWhiteSpace(currentSemester))
            {
                currentSemester = _db.CourseClasses.AsNoTracking()
                    .Where(c => c.Status == CourseClassStatus.Open || c.Status == CourseClassStatus.InProgress)
                    .OrderByDescending(c => c.Semester)
                    .Select(c => c.Semester)
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(currentSemester))
                {
                    currentSemester = _db.CourseClasses.AsNoTracking()
                        .OrderByDescending(c => c.Semester)
                        .Select(c => c.Semester)
                        .FirstOrDefault() ?? "";
                }
            }

            var enrolledApprovedIds = _db.Enrollments.AsNoTracking()
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved)
                .Select(e => e.CourseClassId)
                .ToList();

            var courseClasses = _db.CourseClasses.AsNoTracking()
                .Where(c => c.Semester == currentSemester &&
                            (c.Status == CourseClassStatus.Open || c.Status == CourseClassStatus.InProgress))
                .ToList();

            // preload để tránh query lặp
            var subjectIds = courseClasses.Select(c => c.SubjectId).Distinct().ToList();
            var lecturerIds = courseClasses.Select(c => c.LecturerId).Distinct().ToList();
            var schedules = _db.Schedules.AsNoTracking()
                .Where(s => courseClasses.Select(c => c.Id).Contains(s.CourseClassId))
                .ToList();

            var subjects = _db.Subjects.AsNoTracking()
                .Where(s => subjectIds.Contains(s.Id))
                .ToDictionary(s => s.Id);

            var lecturers = _db.Lecturers.AsNoTracking()
                .Where(l => lecturerIds.Contains(l.Id))
                .ToDictionary(l => l.Id);

            return courseClasses.Select(c =>
            {
                subjects.TryGetValue(c.SubjectId, out var subject);
                lecturers.TryGetValue(c.LecturerId, out var lecturer);

                var scheduleInfo = schedules
                    .Where(s => s.CourseClassId == c.Id)
                    .Select(s => $"{s.DayOfWeek}: {s.StartTime}-{s.EndTime} ({s.Room})")
                    .ToList();

                var canEnroll = CanEnroll(studentId, c.Id, out var msg);
                var remaining = c.MaxStudents - c.CurrentStudents;

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
                    ScheduleInfo = scheduleInfo,
                    CanEnroll = canEnroll && remaining > 0,
                    EnrollmentMessage = remaining <= 0 ? "Class is full" : msg
                };
            }).ToList();
        }

        public bool CanEnroll(int studentId, int courseClassId, out string message)
        {
            var existing = _db.Enrollments.AsNoTracking().FirstOrDefault(e =>
                e.StudentId == studentId &&
                e.CourseClassId == courseClassId &&
                e.Status != EnrollmentStatus.Rejected &&
                e.Status != EnrollmentStatus.Dropped);

            if (existing != null)
            {
                message = "Already enrolled in this class";
                return false;
            }

            var courseClass = _db.CourseClasses.AsNoTracking().FirstOrDefault(c => c.Id == courseClassId);
            if (courseClass == null)
            {
                message = "Class not found";
                return false;
            }

            if (courseClass.CurrentStudents >= courseClass.MaxStudents)
            {
                message = "Class is full";
                return false;
            }

            if (courseClass.Status != CourseClassStatus.Open && courseClass.Status != CourseClassStatus.InProgress)
            {
                message = "Class is not open for registration";
                return false;
            }

            // đã học cùng subject trong cùng kỳ
            var enrolledSameSubject = (
                from e in _db.Enrollments.AsNoTracking()
                join cc in _db.CourseClasses.AsNoTracking() on e.CourseClassId equals cc.Id
                where e.StudentId == studentId
                      && e.Status == EnrollmentStatus.Approved
                      && cc.SubjectId == courseClass.SubjectId
                      && cc.Semester == courseClass.Semester
                select e.Id
            ).Any();

            if (enrolledSameSubject)
            {
                message = "Already enrolled in this subject";
                return false;
            }

            // conflict schedule
            var conflict = HasScheduleConflict(studentId, courseClassId);
            if (conflict.HasConflict)
            {
                message = $"Schedule conflict with {conflict.ConflictingClass}";
                return false;
            }

            // prerequisite
            var subject = _db.Subjects.AsNoTracking().FirstOrDefault(s => s.Id == courseClass.SubjectId);
            if (subject != null && subject.PrerequisiteSubjectIds.Any())
            {
                var passedSubjectIds =
                    (from e in _db.Enrollments.AsNoTracking()
                     join g in _db.Grades.AsNoTracking() on e.Id equals g.EnrollmentId
                     join cc in _db.CourseClasses.AsNoTracking() on e.CourseClassId equals cc.Id
                     where e.StudentId == studentId
                           && e.Status == EnrollmentStatus.Approved
                           && g.IsPassed
                     select cc.SubjectId)
                    .Distinct()
                    .ToList();

                var missing = subject.PrerequisiteSubjectIds.Where(pid => !passedSubjectIds.Contains(pid)).ToList();
                if (missing.Any())
                {
                    var missingCodes = _db.Subjects.AsNoTracking()
                        .Where(s => missing.Contains(s.Id))
                        .Select(s => s.SubjectCode)
                        .ToList();

                    message = $"Prerequisites not met: {string.Join(", ", missingCodes)}";
                    return false;
                }
            }

            message = "Available";
            return true;
        }

        private (bool HasConflict, string ConflictingClass) HasScheduleConflict(int studentId, int courseClassId)
        {
            var newSchedules = _db.Schedules.AsNoTracking()
                .Where(s => s.CourseClassId == courseClassId)
                .ToList();

            if (!newSchedules.Any()) return (false, "");

            var enrolledClassIds = _db.Enrollments.AsNoTracking()
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved)
                .Select(e => e.CourseClassId)
                .ToList();

            if (!enrolledClassIds.Any()) return (false, "");

            var enrolledSchedules = _db.Schedules.AsNoTracking()
                .Where(s => enrolledClassIds.Contains(s.CourseClassId))
                .ToList();

            foreach (var n in newSchedules)
            {
                foreach (var ex in enrolledSchedules)
                {
                    if (n.DayOfWeek == ex.DayOfWeek && n.Period == ex.Period)
                    {
                        var conflictClass = _db.CourseClasses.AsNoTracking()
                            .FirstOrDefault(c => c.Id == ex.CourseClassId);
                        return (true, conflictClass?.ClassCode ?? "Unknown");
                    }

                    if (n.DayOfWeek == ex.DayOfWeek)
                    {
                        if (IsTimeOverlap(n.StartTime, n.EndTime, ex.StartTime, ex.EndTime))
                        {
                            var conflictClass = _db.CourseClasses.AsNoTracking()
                                .FirstOrDefault(c => c.Id == ex.CourseClassId);
                            return (true, conflictClass?.ClassCode ?? "Unknown");
                        }
                    }
                }
            }

            return (false, "");
        }

        private bool IsTimeOverlap(string start1, string end1, string start2, string end2)
        {
            if (!TimeSpan.TryParse(start1, out var s1) ||
                !TimeSpan.TryParse(end1, out var e1) ||
                !TimeSpan.TryParse(start2, out var s2) ||
                !TimeSpan.TryParse(end2, out var e2))
                return false;

            return s1 < e2 && s2 < e1;
        }
    }
}
