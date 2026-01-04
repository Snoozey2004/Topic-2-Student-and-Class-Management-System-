using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service quản lý Notification (SQL Server + EF Core)
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
        private readonly ApplicationDbContext _db;

        public NotificationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<NotificationListViewModel> GetByUserId(int userId)
        {
            var data = _db.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Message,
                    n.Type,
                    n.IsRead,
                    n.LinkUrl,
                    n.CreatedDate
                })
                .ToList(); // ✅ EF chạy xong tại đây
            return data.Select(n => new NotificationListViewModel
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type.ToString(),
                IsRead = n.IsRead,
                LinkUrl = n.LinkUrl,
                CreatedDate = n.CreatedDate,
                TimeAgo = GetTimeAgo(n.CreatedDate) // ✅ chạy ở memory
            }).ToList();
        }


        public NotificationBadgeViewModel GetBadgeInfo(int userId)
        {
            var unreadCount = GetUnreadCount(userId);

            var data = _db.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .Take(5)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Message,
                    n.Type,
                    n.IsRead,
                    n.LinkUrl,
                    n.CreatedDate
                })
                .ToList(); // ✅ EF chạy xong tại đây

            var recentNotifications = data.Select(n => new NotificationListViewModel
            {
                Id = n.Id,
                Title = n.Title,
                Message = (n.Message != null && n.Message.Length > 50)
                            ? n.Message.Substring(0, 50) + "..."
                            : (n.Message ?? ""),
                Type = n.Type.ToString(),
                IsRead = n.IsRead,
                LinkUrl = n.LinkUrl,
                CreatedDate = n.CreatedDate,
                TimeAgo = GetTimeAgo(n.CreatedDate) // ✅ chạy ở memory
            }).ToList();

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
                // KHÔNG set Id nếu Id là Identity (SQL tự tăng)
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                LinkUrl = linkUrl,
                CreatedDate = DateTime.Now
            };

            _db.Notifications.Add(notification);
            _db.SaveChanges();
            return true;
        }

        public bool MarkAsRead(int notificationId)
        {
            var notification = _db.Notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification == null) return false;

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadDate = DateTime.Now;
                _db.SaveChanges();
            }

            return true;
        }

        public bool MarkAllAsRead(int userId)
        {
            var notifications = _db.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToList();

            if (notifications.Count == 0) return true;

            var now = DateTime.Now;
            foreach (var n in notifications)
            {
                n.IsRead = true;
                n.ReadDate = now;
            }

            _db.SaveChanges();
            return true;
        }

        public bool Delete(int notificationId)
        {
            var notification = _db.Notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification == null) return false;

            _db.Notifications.Remove(notification);
            _db.SaveChanges();
            return true;
        }

        public int GetUnreadCount(int userId)
        {
            return _db.Notifications.Count(n => n.UserId == userId && !n.IsRead);
        }

        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Vừa xong";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} phút trước";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} giờ trước";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} ngày trước";
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} tuần trước";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} tháng trước";

            return $"{(int)(timeSpan.TotalDays / 365)} năm trước";
        }
    }
}
