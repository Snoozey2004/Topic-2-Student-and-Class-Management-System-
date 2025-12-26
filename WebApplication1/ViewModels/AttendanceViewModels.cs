using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    /// <summary>
    /// ViewModel cho session ði?m danh
    /// </summary>
    public class AttendanceSessionViewModel
    {
        public int CourseClassId { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public string Session { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public List<StudentAttendanceViewModel> Students { get; set; } = new();
    }

    /// <summary>
    /// ViewModel cho ði?m danh t?ng sinh viên
    /// </summary>
    public class StudentAttendanceViewModel
    {
        public int StudentId { get; set; }
        public int EnrollmentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsPresent { get; set; } = true;
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
        public string? Note { get; set; }
    }

    /// <summary>
    /// ViewModel cho form ði?m danh
    /// </summary>
    public class TakeAttendanceViewModel
    {
        [Required]
        public int CourseClassId { get; set; }
        
        [Required]
        public DateTime SessionDate { get; set; }
        
        [Required]
        public string Session { get; set; } = string.Empty;
        
        public List<StudentAttendanceViewModel> Students { get; set; } = new();
    }

    /// <summary>
    /// ViewModel cho l?ch s? ði?m danh
    /// </summary>
    public class AttendanceHistoryViewModel
    {
        public int CourseClassId { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public List<AttendanceRecordViewModel> Records { get; set; } = new();
        public Dictionary<int, AttendanceStatViewModel> StudentStats { get; set; } = new();
    }

    /// <summary>
    /// ViewModel cho b?n ghi ði?m danh theo bu?i
    /// </summary>
    public class AttendanceRecordViewModel
    {
        public DateTime SessionDate { get; set; }
        public string Session { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public double AttendanceRate { get; set; }
    }

    /// <summary>
    /// ViewModel cho th?ng kê ði?m danh sinh viên
    /// </summary>
    public class AttendanceStatViewModel
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int TotalSessions { get; set; }
        public int PresentSessions { get; set; }
        public int AbsentSessions { get; set; }
        public double AttendanceRate { get; set; }
        public double AttendanceScore { get; set; }
    }

    /// <summary>
    /// ViewModel cho danh sách l?p ð? ði?m danh
    /// </summary>
    public class AttendanceClassListViewModel
    {
        public int Id { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public int TotalSessions { get; set; }
        public double AverageAttendanceRate { get; set; }
    }
}
