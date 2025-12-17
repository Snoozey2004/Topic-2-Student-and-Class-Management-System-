using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    /// <summary>
    /// ViewModel cho danh sách thông báo
    /// </summary>
    public class NotificationListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public string? LinkUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel cho gửi thông báo lớp
    /// </summary>
    public class SendClassNotificationViewModel
    {
        public int CourseClassId { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel cho header notification badge
    /// </summary>
    public class NotificationBadgeViewModel
    {
        public int UnreadCount { get; set; }
        public List<NotificationListViewModel> RecentNotifications { get; set; } = new List<NotificationListViewModel>();
    }
}
