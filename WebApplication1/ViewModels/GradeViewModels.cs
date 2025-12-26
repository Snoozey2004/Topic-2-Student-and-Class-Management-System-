using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    /// <summary>
    /// ViewModel cho nhập điểm
    /// </summary>
    public class GradeInputViewModel
    {
        public int GradeId { get; set; }
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;

        [Range(0, 10, ErrorMessage = "Score must be between 0-10")]
        public double? AttendanceScore { get; set; }

        [Range(0, 10, ErrorMessage = "Score must be between 0-10")]
        public double? MidtermScore { get; set; }

        [Range(0, 10, ErrorMessage = "Score must be between 0-10")]
        public double? FinalScore { get; set; }
    }

    /// <summary>
    /// ViewModel cho form tạo/sửa Grade
    /// </summary>
    public class GradeFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Enrollment is required")]
        public int EnrollmentId { get; set; }

        public int StudentId { get; set; }
        public int CourseClassId { get; set; }

        [Range(0, 10, ErrorMessage = "Score must be between 0-10")]
        public double? AttendanceScore { get; set; }

        [Range(0, 10, ErrorMessage = "Score must be between 0-10")]
        public double? MidtermScore { get; set; }

        [Range(0, 10, ErrorMessage = "Score must be between 0-10")]
        public double? FinalScore { get; set; }
    }

    /// <summary>
    /// ViewModel cho danh sách Grade (Admin)
    /// </summary>
    public class GradeListViewModel
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string CourseClassCode { get; set; } = string.Empty;
        public double? TotalScore { get; set; }
        public string? LetterGrade { get; set; }
    }

    /// <summary>
    /// ViewModel cho chi tiết Grade (Admin)
    /// </summary>
    public class GradeDetailAdminViewModel
    {
        public int Id { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public double? AttendanceScore { get; set; }
        public double? MidtermScore { get; set; }
        public double? FinalScore { get; set; }
        public double? TotalScore { get; set; }
        public string? LetterGrade { get; set; }
        public bool IsPassed { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    /// <summary>
    /// ViewModel cho danh sách điểm của lớp
    /// </summary>
    public class ClassGradeListViewModel
    {
        public int CourseClassId { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public List<GradeInputViewModel> Grades { get; set; } = new List<GradeInputViewModel>();
    }

    /// <summary>
    /// ViewModel cho bảng điểm của sinh viên
    /// </summary>
    public class StudentGradeViewModel
    {
        public string StudentCode { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public List<GradeDetailViewModel> Grades { get; set; } = new List<GradeDetailViewModel>();
        public double? GPA { get; set; }
        public int TotalCredits { get; set; }
        public int PassedCredits { get; set; }
    }

    /// <summary>
    /// ViewModel cho chi tiết điểm từng môn
    /// </summary>
    public class GradeDetailViewModel
    {
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public double? AttendanceScore { get; set; }
        public double? MidtermScore { get; set; }
        public double? FinalScore { get; set; }
        public double? TotalScore { get; set; }
        public string? LetterGrade { get; set; }
        public bool IsPassed { get; set; }
    }
}
