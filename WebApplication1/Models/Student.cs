namespace WebApplication1.Models
{
    /// <summary>
    /// Model ??i di?n cho sinh viên
    /// </summary>
    public class Student
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int? AdministrativeClassId { get; set; } // L?p hành chính
        public string Major { get; set; } = string.Empty; // Chuyên ngành
        public int AdmissionYear { get; set; } // N?m nh?p h?c
        public DateTime CreatedDate { get; set; }
        // Navigation
        public User User { get; set; } = null!;
        public AdministrativeClass? AdministrativeClass { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }
}
