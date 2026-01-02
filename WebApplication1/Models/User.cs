namespace WebApplication1.Models
{
    /// <summary>
    /// Model ??i di?n cho ng??i dùng trong h? th?ng
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        // Navigation
        public Student? Student { get; set; }
        public Lecturer? Lecturer { get; set; }
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<Enrollment> ApprovedEnrollments { get; set; } = new List<Enrollment>(); // ApprovedBy

    }
  

    public enum UserRole
    {
        Admin,
        Lecturer,
        Student
    }

    public enum UserStatus
    {
        Pending,
        Active,
        Locked
    }
}
