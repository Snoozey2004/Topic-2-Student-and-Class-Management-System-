using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    /// <summary>
    /// ViewModel cho danh sách Enrollment
    /// </summary>
    public class EnrollmentListViewModel
    {
        public int Id { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
    }

    /// <summary>
    /// ViewModel cho form t?o/s?a Enrollment
    /// </summary>
    public class EnrollmentFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Student is required")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Course Class is required")]
        public int CourseClassId { get; set; }
    }

    /// <summary>
    /// ViewModel cho ??ng ký môn h?c (Student)
    /// </summary>
    public class EnrollmentRegisterViewModel
    {
        public List<AvailableCourseViewModel> AvailableCourses { get; set; } = new List<AvailableCourseViewModel>();
        public List<EnrollmentListViewModel> MyEnrollments { get; set; } = new List<EnrollmentListViewModel>();
    }

    /// <summary>
    /// ViewModel cho môn h?c có th? ??ng ký
    /// </summary>
    public class AvailableCourseViewModel
    {
        public int CourseClassId { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string LecturerName { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public int CurrentStudents { get; set; }
        public int MaxStudents { get; set; }
        public string Room { get; set; } = string.Empty;
        public List<string> ScheduleInfo { get; set; } = new List<string>();
        public bool CanEnroll { get; set; }
        public string? EnrollmentMessage { get; set; }
    }

    /// <summary>
    /// ViewModel cho duy?t/t? ch?i ??ng ký
    /// </summary>
    public class EnrollmentApprovalViewModel
    {
        public int EnrollmentId { get; set; }
        public EnrollmentStatus Status { get; set; }
        public string? RejectionReason { get; set; }
    }
}
