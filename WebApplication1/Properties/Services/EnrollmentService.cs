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
                .Where(e => e.StudentId == studentId && 
                           e.Status != EnrollmentStatus.Dropped &&
                           e.Status != EnrollmentStatus.Rejected)
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

        public bool Create(EnrollmentFormViewModel model)
        {
            var existing = FakeDatabase.Enrollments.FirstOrDefault(e =>
                e.StudentId == model.StudentId &&
                e.CourseClassId == model.CourseClassId &&
                e.Status != EnrollmentStatus.Rejected &&
                e.Status != EnrollmentStatus.Dropped);

            if (existing != null) return false;

            var enrollment = new Enrollment
            {
                Id = FakeDatabase.GetNextEnrollmentId(),
                StudentId = model.StudentId,
                CourseClassId = model.CourseClassId,
                EnrollmentDate = DateTime.Now,
                Status = EnrollmentStatus.Pending
            };

            FakeDatabase.Enrollments.Add(enrollment);
            return true;
        }

        public bool Update(EnrollmentFormViewModel model)
        {
            var enrollment = GetById(model.Id ?? 0);
            if (enrollment == null) return false;

            enrollment.StudentId = model.StudentId;
            enrollment.CourseClassId = model.CourseClassId;
            return true;
        }

        public bool Delete(int id)
        {
            var enrollment = GetById(id);
            if (enrollment == null) return false;

            if (enrollment.Status == EnrollmentStatus.Approved)
            {
                var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == enrollment.CourseClassId);
                if (courseClass != null && courseClass.CurrentStudents > 0)
                {
                    courseClass.CurrentStudents--;
                }

                var grade = FakeDatabase.Grades.FirstOrDefault(g => g.EnrollmentId == enrollment.Id);
                if (grade != null)
                {
                    FakeDatabase.Grades.Remove(grade);
                }
            }

            FakeDatabase.Enrollments.Remove(enrollment);
            return true;
        }

        public bool Enroll(int studentId, int courseClassId, bool autoApprove = true)
        {
            if (!CanEnroll(studentId, courseClassId, out string message))
            {
                return false;
            }

            var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == courseClassId);
            if (courseClass == null) return false;

            var enrollment = new Enrollment
            {
                Id = FakeDatabase.GetNextEnrollmentId(),
                StudentId = studentId,
                CourseClassId = courseClassId,
                EnrollmentDate = DateTime.Now,
                Status = autoApprove ? EnrollmentStatus.Approved : EnrollmentStatus.Pending
            };

            // Auto-approve: immediately increase student count and create grade
            if (autoApprove)
            {
                enrollment.ApprovedDate = DateTime.Now;
                enrollment.ApprovedBy = 0; // System auto-approve
                
                // Increase current students count
                courseClass.CurrentStudents++;

                // Create Grade record
                var grade = new Grade
                {
                    Id = FakeDatabase.GetNextGradeId(),
                    EnrollmentId = enrollment.Id,
                    StudentId = studentId,
                    CourseClassId = courseClassId,
                    IsPassed = false
                };
                FakeDatabase.Grades.Add(grade);
            }

            FakeDatabase.Enrollments.Add(enrollment);

            // Notify student
            var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == studentId);
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
            var enrollment = GetById(enrollmentId);
            if (enrollment == null || enrollment.Status != EnrollmentStatus.Pending)
            {
                return false;
            }

            enrollment.Status = EnrollmentStatus.Approved;
            enrollment.ApprovedDate = DateTime.Now;
            enrollment.ApprovedBy = approvedBy;

            var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == enrollment.CourseClassId);
            if (courseClass != null)
            {
                courseClass.CurrentStudents++;
            }

            // Create Grade record
            var grade = new Grade
            {
                Id = FakeDatabase.GetNextGradeId(),
                EnrollmentId = enrollment.Id,
                StudentId = enrollment.StudentId,
                CourseClassId = enrollment.CourseClassId,
                IsPassed = false
            };
            FakeDatabase.Grades.Add(grade);

            var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == enrollment.StudentId);
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
            var enrollment = GetById(enrollmentId);
            if (enrollment == null || enrollment.Status != EnrollmentStatus.Pending)
            {
                return false;
            }

            enrollment.Status = EnrollmentStatus.Rejected;
            enrollment.RejectionReason = reason;

            var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == enrollment.StudentId);
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
            var enrollment = GetById(enrollmentId);
            if (enrollment == null) return false;

            var wasApproved = enrollment.Status == EnrollmentStatus.Approved;
            enrollment.Status = EnrollmentStatus.Dropped;

            if (wasApproved)
            {
                var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == enrollment.CourseClassId);
                if (courseClass != null && courseClass.CurrentStudents > 0)
                {
                    courseClass.CurrentStudents--;
                }

                // Remove grade record
                var grade = FakeDatabase.Grades.FirstOrDefault(g => g.EnrollmentId == enrollmentId);
                if (grade != null)
                {
                    FakeDatabase.Grades.Remove(grade);
                }
            }

            return true;
        }

        /// <summary>
        /// Get available courses for student - semester is passed from Controller (no hardcode)
        /// </summary>
        public List<AvailableCourseViewModel> GetAvailableCoursesForStudent(int studentId, string? semester = null)
        {
            // L?y semester t? parameter, n?u null thì l?y t? database
            var currentSemester = semester;
            if (string.IsNullOrEmpty(currentSemester))
            {
                currentSemester = FakeDatabase.CourseClasses
                    .Where(c => c.Status == CourseClassStatus.Open || c.Status == CourseClassStatus.InProgress)
                    .OrderByDescending(c => c.Semester)
                    .FirstOrDefault()?.Semester;
                
                if (string.IsNullOrEmpty(currentSemester))
                {
                    currentSemester = FakeDatabase.CourseClasses
                        .OrderByDescending(c => c.Semester)
                        .FirstOrDefault()?.Semester ?? "";
                }
            }

            var enrolledCourseClassIds = FakeDatabase.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved)
                .Select(e => e.CourseClassId)
                .ToList();

            return FakeDatabase.CourseClasses
                .Where(c => c.Semester == currentSemester && 
                           (c.Status == CourseClassStatus.Open || c.Status == CourseClassStatus.InProgress))
                .Select(c =>
                {
                    var subject = FakeDatabase.Subjects.FirstOrDefault(s => s.Id == c.SubjectId);
                    var lecturer = FakeDatabase.Lecturers.FirstOrDefault(l => l.Id == c.LecturerId);
                    var schedules = FakeDatabase.Schedules
                        .Where(s => s.CourseClassId == c.Id)
                        .Select(s => $"{s.DayOfWeek}: {s.StartTime}-{s.EndTime} ({s.Room})")
                        .ToList();

                    var canEnroll = CanEnroll(studentId, c.Id, out string message);
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
                        ScheduleInfo = schedules,
                        CanEnroll = canEnroll && remaining > 0,
                        EnrollmentMessage = remaining <= 0 ? "Class is full" : message
                    };
                }).ToList();
        }

        public bool CanEnroll(int studentId, int courseClassId, out string message)
        {
            var existingEnrollment = FakeDatabase.Enrollments.FirstOrDefault(e =>
                e.StudentId == studentId &&
                e.CourseClassId == courseClassId &&
                e.Status != EnrollmentStatus.Rejected &&
                e.Status != EnrollmentStatus.Dropped);

            if (existingEnrollment != null)
            {
                message = "Already enrolled in this class";
                return false;
            }

            var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == courseClassId);
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

            var enrolledInSameSubject = FakeDatabase.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved)
                .Any(e => 
                {
                    var cc = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == e.CourseClassId);
                    return cc != null && cc.SubjectId == courseClass.SubjectId && cc.Semester == courseClass.Semester;
                });

            if (enrolledInSameSubject)
            {
                message = "Already enrolled in this subject";
                return false;
            }

            var conflictCheck = HasScheduleConflict(studentId, courseClassId);
            if (conflictCheck.HasConflict)
            {
                message = $"Schedule conflict with {conflictCheck.ConflictingClass}";
                return false;
            }

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
                    var missingSubjectNames = FakeDatabase.Subjects
                        .Where(s => missingPrerequisites.Contains(s.Id))
                        .Select(s => s.SubjectCode)
                        .ToList();
                    message = $"Prerequisites not met: {string.Join(", ", missingSubjectNames)}";
                    return false;
                }
            }

            message = "Available";
            return true;
        }

        private (bool HasConflict, string ConflictingClass) HasScheduleConflict(int studentId, int courseClassId)
        {
            var newClassSchedules = FakeDatabase.Schedules
                .Where(s => s.CourseClassId == courseClassId)
                .ToList();

            if (!newClassSchedules.Any())
            {
                return (false, "");
            }

            var enrolledCourseClassIds = FakeDatabase.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Approved)
                .Select(e => e.CourseClassId)
                .ToList();

            var enrolledSchedules = FakeDatabase.Schedules
                .Where(s => enrolledCourseClassIds.Contains(s.CourseClassId))
                .ToList();

            foreach (var newSchedule in newClassSchedules)
            {
                foreach (var existingSchedule in enrolledSchedules)
                {
                    if (newSchedule.DayOfWeek == existingSchedule.DayOfWeek &&
                        newSchedule.Period == existingSchedule.Period)
                    {
                        var conflictingClass = FakeDatabase.CourseClasses
                            .FirstOrDefault(c => c.Id == existingSchedule.CourseClassId);
                        return (true, conflictingClass?.ClassCode ?? "Unknown");
                    }

                    if (newSchedule.DayOfWeek == existingSchedule.DayOfWeek)
                    {
                        if (IsTimeOverlap(newSchedule.StartTime, newSchedule.EndTime,
                                         existingSchedule.StartTime, existingSchedule.EndTime))
                        {
                            var conflictingClass = FakeDatabase.CourseClasses
                                .FirstOrDefault(c => c.Id == existingSchedule.CourseClassId);
                            return (true, conflictingClass?.ClassCode ?? "Unknown");
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
            {
                return false;
            }

            // Two time ranges overlap if: start1 < end2 AND start2 < end1
            return s1 < e2 && s2 < e1;
        }
    }
}
