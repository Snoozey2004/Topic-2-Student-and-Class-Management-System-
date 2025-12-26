namespace WebApplication1.Models
{
    /// <summary>
    /// Model ??i di?n cho ??ng ký môn h?c
    /// </summary>
    public class Enrollment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseClassId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public EnrollmentStatus Status { get; set; }
        public string? RejectionReason { get; set; } // Lý do t? ch?i (n?u có)
        public DateTime? ApprovedDate { get; set; }
        public int? ApprovedBy { get; set; } // UserId c?a Admin duy?t
    }

    public enum EnrollmentStatus
    {
        Pending,   // Ch? duy?t
        Approved,  // ?ã duy?t
        Rejected,  // T? ch?i
        Dropped    // ?ã h?y
    }
}
