namespace WebApplication1.Models
{
    /// <summary>
    /// Model ??i di?n cho gi?ng viên
    /// </summary>
    public class Lecturer
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string LecturerCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty; // Khoa
        public string Title { get; set; } = string.Empty; // Ch?c danh: GS, PGS, TS, ThS
        public string Specialization { get; set; } = string.Empty; // Chuyên môn
        public DateTime JoinDate { get; set; }
    }
}
