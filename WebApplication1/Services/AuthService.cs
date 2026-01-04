using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service xử lý authentication và authorization (SQL Server + EF Core)
    /// </summary>
    public interface IAuthService
    {
        User? Authenticate(string email, string password);
        User? GetUserByEmail(string email);
        User? GetUserById(int id);
        bool Register(string email, string password, string fullName);
        bool ChangePassword(int userId, string currentPassword, string newPassword);
        bool ResetPassword(string email, string newPassword);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;

        public AuthService(ApplicationDbContext db)
        {
            _db = db;
        }

        public User? Authenticate(string email, string password)
        {
            var user = _db.Users.FirstOrDefault(u =>
                u.Email.ToLower() == email.ToLower() &&
                u.Password == password);

            if (user != null && user.Status == UserStatus.Active)
            {
                user.LastLoginDate = DateTime.Now;
                _db.SaveChanges(); // lưu LastLoginDate
                return user;
            }

            return null;
        }

        public User? GetUserByEmail(string email)
        {
            return _db.Users.FirstOrDefault(u =>
                u.Email.ToLower() == email.ToLower());
        }

        public User? GetUserById(int id)
        {
            return _db.Users.FirstOrDefault(u => u.Id == id);
        }

        public bool Register(string email, string password, string fullName)
        {
            // Kiểm tra email đã tồn tại chưa
            if (GetUserByEmail(email) != null)
                return false;

            // Tạo User (KHÔNG set Id - SQL tự tăng)
            var user = new User
            {
                Email = email,
                Password = password,
                FullName = fullName,
                Role = UserRole.Student,
                Status = UserStatus.Pending, // chờ admin duyệt
                CreatedDate = DateTime.Now
            };

            _db.Users.Add(user);
            _db.SaveChanges(); // để lấy user.Id

            // Tạo Student record
            var student = new Student
            {
                UserId = user.Id,
                StudentCode = $"SV{DateTime.Now:yyyyMMddHHmmss}",
                FullName = fullName,
                Email = email,
                DateOfBirth = DateTime.Now.AddYears(-20),
                PhoneNumber = "",
                Address = "",
                Major = "Chưa xác định",
                AdmissionYear = DateTime.Now.Year,
                CreatedDate = DateTime.Now
            };

            _db.Students.Add(student);
            _db.SaveChanges();

            return true;
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = GetUserById(userId);
            if (user == null || user.Password != currentPassword)
                return false;

            user.Password = newPassword;
            _db.SaveChanges();
            return true;
        }

        public bool ResetPassword(string email, string newPassword)
        {
            var user = GetUserByEmail(email);
            if (user == null)
                return false;

            user.Password = newPassword;
            _db.SaveChanges();
            return true;
        }
    }
}
