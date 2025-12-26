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
