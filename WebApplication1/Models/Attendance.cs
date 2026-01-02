namespace WebApplication1.Models
{
    /// <summary>
    /// Model ??i di?n cho ?i?m danh
    /// </summary>
    public class Attendance
    {
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseClassId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Session { get; set; } = string.Empty; // Ca h?c
        public AttendanceStatus Status { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; } // LecturerId
                                           // Navigation
        public Enrollment Enrollment { get; set; } = null!;
        public Student Student { get; set; } = null!;
        public CourseClass CourseClass { get; set; } = null!;
        public Lecturer Lecturer { get; set; } = null!;
    }

    public enum AttendanceStatus
    {
        Present,  // Có m?t
        Absent,   // V?ng
        Late,     // ?i mu?n
        Excused   // V?ng có phép
    }
}
