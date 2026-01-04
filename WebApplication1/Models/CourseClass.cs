namespace WebApplication1.Models
{
    /// <summary>
    /// Model ??i di?n cho l?p h?c ph?n (Course Class)
    /// </summary>
    public class CourseClass
    {
        public int Id { get; set; }
        public string ClassCode { get; set; } = string.Empty; // VD: IT001.01
        public int SubjectId { get; set; }
        public int LecturerId { get; set; }
        public string Semester { get; set; } = string.Empty; // VD: HK1-2024
        public int MaxStudents { get; set; }
        public int CurrentStudents { get; set; }
        public string Room { get; set; } = string.Empty;
        public CourseClassStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation
        public Subject Subject { get; set; } = null!;
        public Lecturer Lecturer { get; set; } = null!;
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }

    public enum CourseClassStatus
    {
        Open,        // M? ??ng ký
        InProgress,  // ?ang h?c
        Completed,   // Hoàn thành
        Cancelled    // H?y
    }
}
