namespace WebApplication1.Models
{
    /// <summary>
    /// Model ??i di?n cho ?i?m s?
    /// </summary>
    public class Grade
    {
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseClassId { get; set; }
        
        // ?i?m thành ph?n
        public double? AttendanceScore { get; set; }    // ?i?m chuyên c?n (10%)
        public double? MidtermScore { get; set; }        // ?i?m gi?a k? (30%)
        public double? FinalScore { get; set; }          // ?i?m cu?i k? (60%)
        
        // ?i?m t?ng k?t
        public double? TotalScore { get; set; }          // ?i?m t?ng (h? 10)
        public string? LetterGrade { get; set; }         // ?i?m ch? (A, B+, B, C+, C, D+, D, F)
        public bool IsPassed { get; set; }               // ??t/Không ??t
        
        public DateTime? LastUpdated { get; set; }
        public int? UpdatedBy { get; set; } // LecturerId c?p nh?t ?i?m
    }
}
