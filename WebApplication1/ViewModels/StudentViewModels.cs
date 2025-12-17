using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    /// <summary>
    /// ViewModel cho t?o/ch?nh s?a Student
    /// </summary>
    public class StudentFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Mã sinh viên là b?t bu?c")]
        public string StudentCode { get; set; } = string.Empty;

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

        public string Address { get; set; } = string.Empty;

        public int? AdministrativeClassId { get; set; }

        [Required(ErrorMessage = "Chuyên ngành là b?t bu?c")]
        public string Major { get; set; } = string.Empty;

        [Required(ErrorMessage = "N?m nh?p h?c là b?t bu?c")]
        public int AdmissionYear { get; set; }

        // Cho create user m?i
        public string? Password { get; set; }
    }

    /// <summary>
    /// ViewModel cho danh sách Student
    /// </summary>
    public class StudentListViewModel
    {
        public int Id { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Major { get; set; } = string.Empty;
        public int AdmissionYear { get; set; }
        public string? AdministrativeClassName { get; set; }
    }

    /// <summary>
    /// ViewModel cho chi ti?t Student
    /// </summary>
    public class StudentDetailViewModel
    {
        public int Id { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Major { get; set; } = string.Empty;
        public int AdmissionYear { get; set; }
        public string? AdministrativeClassName { get; set; }
        public List<EnrollmentInfoViewModel> Enrollments { get; set; } = new List<EnrollmentInfoViewModel>();
        public double? GPA { get; set; }
    }

    /// <summary>
    /// ViewModel cho thông tin enrollment trong student detail
    /// </summary>
    public class EnrollmentInfoViewModel
    {
        public string SubjectName { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double? TotalScore { get; set; }
        public string? LetterGrade { get; set; }
    }
}
