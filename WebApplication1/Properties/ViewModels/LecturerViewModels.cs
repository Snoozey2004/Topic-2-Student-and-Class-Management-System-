using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    /// <summary>
    /// ViewModel cho t?o/ch?nh s?a Lecturer
    /// </summary>
    public class LecturerFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Mã gi?ng viên là b?t bu?c")]
        public string LecturerCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "H? và tên là b?t bu?c")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là b?t bu?c")]
        [EmailAddress(ErrorMessage = "Email không h?p l?")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh là b?t bu?c")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "S? ?i?n tho?i là b?t bu?c")]
        [Phone(ErrorMessage = "S? ?i?n tho?i không h?p l?")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Khoa là b?t bu?c")]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ch?c danh là b?t bu?c")]
        public string Title { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày vào làm là b?t bu?c")]
        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }

        // Cho create user m?i
        public string? Password { get; set; }
    }

    /// <summary>
    /// ViewModel cho danh sách Lecturer
    /// </summary>
    public class LecturerListViewModel
    {
        public int Id { get; set; }
        public string LecturerCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel cho chi ti?t Lecturer
    /// </summary>
    public class LecturerDetailViewModel
    {
        public int Id { get; set; }
        public string LecturerCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public List<CourseClassInfoViewModel> TeachingClasses { get; set; } = new List<CourseClassInfoViewModel>();
    }

    /// <summary>
    /// ViewModel cho thông tin course class
    /// </summary>
    public class CourseClassInfoViewModel
    {
        public int Id { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public int CurrentStudents { get; set; }
        public int MaxStudents { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
