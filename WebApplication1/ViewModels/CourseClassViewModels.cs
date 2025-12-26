using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    /// <summary>
    /// ViewModel cho t?o/ch?nh s?a CourseClass
    /// </summary>
    public class CourseClassFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Mã l?p h?c ph?n là b?t bu?c")]
        public string ClassCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Môn h?c là b?t bu?c")]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "Gi?ng viên là b?t bu?c")]
        public int LecturerId { get; set; }

        [Required(ErrorMessage = "H?c k? là b?t bu?c")]
        public string Semester { get; set; } = string.Empty;

        [Required(ErrorMessage = "S? sinh viên t?i ?a là b?t bu?c")]
        [Range(1, 200, ErrorMessage = "S? sinh viên t?i ?a ph?i t? 1-200")]
        public int MaxStudents { get; set; }

        [Required(ErrorMessage = "Phòng h?c là b?t bu?c")]
        public string Room { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tr?ng thái là b?t bu?c")]
        public CourseClassStatus Status { get; set; }
    }

    /// <summary>
    /// ViewModel cho danh sách CourseClass
    /// </summary>
    public class CourseClassListViewModel
    {
        public int Id { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public string LecturerName { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public int CurrentStudents { get; set; }
        public int MaxStudents { get; set; }
        public string Room { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel cho chi ti?t CourseClass
    /// </summary>
    public class CourseClassDetailViewModel
    {
        public int Id { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string LecturerName { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public int CurrentStudents { get; set; }
        public int MaxStudents { get; set; }
        public string Room { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<ScheduleInfoViewModel> Schedules { get; set; } = new List<ScheduleInfoViewModel>();
        public List<StudentInClassViewModel> Students { get; set; } = new List<StudentInClassViewModel>();
    }

    /// <summary>
    /// ViewModel cho thông tin l?ch h?c
    /// </summary>
    public class ScheduleInfoViewModel
    {
        public int Id { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string Session { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public string TimeRange { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel cho sinh viên trong l?p
    /// </summary>
    public class StudentInClassViewModel
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EnrollmentStatus { get; set; } = string.Empty;
        public double? TotalScore { get; set; }
        public string? LetterGrade { get; set; }
    }
}
