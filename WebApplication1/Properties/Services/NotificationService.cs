using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service qu?n lý Notification
    /// </summary>
    public interface INotificationService
    {
        List<NotificationListViewModel> GetByUserId(int userId);
        NotificationBadgeViewModel GetBadgeInfo(int userId);
        bool CreateNotification(int userId, string title, string message, NotificationType type, string? linkUrl = null);
        bool MarkAsRead(int notificationId);
        bool MarkAllAsRead(int userId);
        bool Delete(int notificationId);
        int GetUnreadCount(int userId);
    }

    public class NotificationService : INotificationService
    {
        public List<NotificationListViewModel> GetByUserId(int userId)
        {
            return FakeDatabase.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .Select(n => new NotificationListViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type.ToString(),
                    IsRead = n.IsRead,
                    LinkUrl = n.LinkUrl,
                    CreatedDate = n.CreatedDate,
                    TimeAgo = GetTimeAgo(n.CreatedDate)
                })
                .ToList();
        }

        public NotificationBadgeViewModel GetBadgeInfo(int userId)
        {
            var unreadCount = GetUnreadCount(userId);
            var recentNotifications = FakeDatabase.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .Take(5)
                .Select(n => new NotificationListViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message.Length > 50 ? n.Message.Substring(0, 50) + "..." : n.Message,
                    Type = n.Type.ToString(),
                    IsRead = n.IsRead,
                    LinkUrl = n.LinkUrl,
                    CreatedDate = n.CreatedDate,
                    TimeAgo = GetTimeAgo(n.CreatedDate)
                })
                .ToList();

            return new NotificationBadgeViewModel
            {
                UnreadCount = unreadCount,
                RecentNotifications = recentNotifications
            };
        }

        public bool CreateNotification(int userId, string title, string message, NotificationType type, string? linkUrl = null)
        {
            var notification = new Notification
            {
                Id = FakeDatabase.GetNextNotificationId(),
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                LinkUrl = linkUrl,
                CreatedDate = DateTime.Now
            };

            FakeDatabase.Notifications.Add(notification);
            return true;
        }

        public bool MarkAsRead(int notificationId)
        {
            var notification = FakeDatabase.Notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification == null) return false;

            notification.IsRead = true;
            notification.ReadDate = DateTime.Now;
            return true;
        }

        public bool MarkAllAsRead(int userId)
        {
            var notifications = FakeDatabase.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToList();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadDate = DateTime.Now;
            }

            return true;
        }

        public bool Delete(int notificationId)
        {
            var notification = FakeDatabase.Notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification == null) return false;

            FakeDatabase.Notifications.Remove(notification);
            return true;
        }

        public int GetUnreadCount(int userId)
        {
            return FakeDatabase.Notifications.Count(n => n.UserId == userId && !n.IsRead);
        }

        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "V?a xong";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} phút tr??c";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} gi? tr??c";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} ngày tr??c";
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} tu?n tr??c";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} tháng tr??c";

            return $"{(int)(timeSpan.TotalDays / 365)} n?m tr??c";
        }
    }
}
