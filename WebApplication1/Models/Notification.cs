namespace WebApplication1.Models
{
    /// <summary>
    /// Model ??i di?n cho thông báo
    /// </summary>
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public string? LinkUrl { get; set; } // URL liên k?t (n?u có)
        public DateTime CreatedDate { get; set; }
        public DateTime? ReadDate { get; set; }
    }

    public enum NotificationType
    {
        Enrollment,        // ??ng ký môn h?c
        Schedule,          // L?ch h?c
        Grade,             // ?i?m s?
        ClassAnnouncement, // Thông báo l?p
        System             // H? th?ng
    }
}
