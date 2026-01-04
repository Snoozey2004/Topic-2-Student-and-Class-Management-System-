using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service quản lý User (SQL Server + EF Core)
    /// </summary>
    public interface IUserService
    {
        List<UserListViewModel> GetAll();
        User? GetById(int id);
        bool Create(UserFormViewModel model);
        bool Update(UserFormViewModel model);
        bool Delete(int id);
        bool ChangeUserRole(int userId, UserRole newRole);
        bool ChangeUserStatus(int userId, UserStatus newStatus);
        bool UpdateProfile(int userId, string fullName);
        bool ChangePassword(int userId, string currentPassword, string newPassword);
        bool ValidatePassword(int userId, string password);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;

        public UserService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<UserListViewModel> GetAll()
        {
            return _db.Users
                .Select(u => new UserListViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FullName,
                    Role = u.Role,
                    Status = u.Status,
                    CreatedDate = u.CreatedDate,
                    LastLoginDate = u.LastLoginDate
                })
                .OrderByDescending(u => u.CreatedDate)
                .ToList();
        }

        public User? GetById(int id)
        {
            return _db.Users.FirstOrDefault(u => u.Id == id);
        }

        public bool Create(UserFormViewModel model)
        {
            // Kiểm tra email đã tồn tại
            var emailLower = model.Email.Trim().ToLower();
            if (_db.Users.Any(u => u.Email.ToLower() == emailLower))
                return false;

            var user = new User
            {
                // KHÔNG set Id - SQL tự tăng
                Email = model.Email.Trim(),
                Password = string.IsNullOrWhiteSpace(model.Password) ? "123456" : model.Password,
                FullName = model.FullName,
                Role = model.Role,
                Status = model.Status,
                CreatedDate = DateTime.Now
            };

            _db.Users.Add(user);
            _db.SaveChanges();
            return true;
        }

        public bool Update(UserFormViewModel model)
        {
            var id = model.Id ?? 0;
            var user = GetById(id);
            if (user == null) return false;

            // Kiểm tra email trùng (ngoại trừ user hiện tại)
            var emailLower = model.Email.Trim().ToLower();
            if (_db.Users.Any(u => u.Id != user.Id && u.Email.ToLower() == emailLower))
                return false;

            user.Email = model.Email.Trim();
            user.FullName = model.FullName;
            user.Role = model.Role;
            user.Status = model.Status;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                user.Password = model.Password!;
            }

            _db.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var user = GetById(id);
            if (user == null) return false;

            _db.Users.Remove(user);
            _db.SaveChanges();
            return true;
        }

        public bool ChangeUserRole(int userId, UserRole newRole)
        {
            var user = GetById(userId);
            if (user == null) return false;

            user.Role = newRole;
            _db.SaveChanges();
            return true;
        }

        public bool ChangeUserStatus(int userId, UserStatus newStatus)
        {
            var user = GetById(userId);
            if (user == null) return false;

            user.Status = newStatus;
            _db.SaveChanges();
            return true;
        }

        public bool UpdateProfile(int userId, string fullName)
        {
            var user = GetById(userId);
            if (user == null) return false;

            user.FullName = fullName;
            _db.SaveChanges();
            return true;
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = GetById(userId);
            if (user == null) return false;

            if (user.Password != currentPassword)
                return false;

            user.Password = newPassword;
            _db.SaveChanges();
            return true;
        }

        public bool ValidatePassword(int userId, string password)
        {
            var user = GetById(userId);
            if (user == null) return false;

            return user.Password == password;
        }
    }
}
