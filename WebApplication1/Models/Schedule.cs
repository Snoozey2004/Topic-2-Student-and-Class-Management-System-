namespace WebApplication1.Models
{
    /// <summary>
    /// Model ??i di?n cho l?ch h?c
    /// </summary>
    public class Schedule
    {
        public int Id { get; set; }
        public int CourseClassId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string Session { get; set; } = string.Empty; // Sáng, Chi?u, T?i
        public string Period { get; set; } = string.Empty; // Ca 1-3, Ca 4-6, Ca 7-9
        public string StartTime { get; set; } = string.Empty; // VD: 07:00
        public string EndTime { get; set; } = string.Empty; // VD: 09:00
        public string Room { get; set; } = string.Empty;
        public DateTime EffectiveDate { get; set; } // Ngày b?t ??u áp d?ng
        public DateTime? EndDate { get; set; } // Ngày k?t thúc (null = vô th?i h?n)
        public DateTime CreatedDate { get; set; }
        // Navigation
        public CourseClass CourseClass { get; set; } = null!;
    }

    public enum SessionType
    {
        Morning,   // Sáng
        Afternoon, // Chi?u
        Evening    // T?i
    }
}
