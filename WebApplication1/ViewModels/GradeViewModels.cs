using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    /// <summary>
    /// ViewModel cho nh?p ?i?m
    /// </summary>
    public class GradeInputViewModel
    {
        public int GradeId { get; set; }
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;

        [Range(0, 10, ErrorMessage = "?i?m ph?i t? 0-10")]
        public double? AttendanceScore { get; set; }

        [Range(0, 10, ErrorMessage = "?i?m ph?i t? 0-10")]
        public double? MidtermScore { get; set; }

        [Range(0, 10, ErrorMessage = "?i?m ph?i t? 0-10")]
        public double? FinalScore { get; set; }
    }

    /// <summary>
    /// ViewModel cho danh sách ?i?m c?a l?p
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
    /// ViewModel cho b?ng ?i?m c?a sinh viên
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
    /// ViewModel cho chi ti?t ?i?m t?ng môn
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
