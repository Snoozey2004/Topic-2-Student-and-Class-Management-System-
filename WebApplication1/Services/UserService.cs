using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service qu?n lý User
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
        public List<UserListViewModel> GetAll()
        {
            return FakeDatabase.Users.Select(u => new UserListViewModel
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role,
                Status = u.Status,
                CreatedDate = u.CreatedDate,
                LastLoginDate = u.LastLoginDate
            }).OrderByDescending(u => u.CreatedDate).ToList();
        }

        public User? GetById(int id)
        {
            return FakeDatabase.Users.FirstOrDefault(u => u.Id == id);
        }

        public bool Create(UserFormViewModel model)
        {
            // Ki?m tra email ?ã t?n t?i
            if (FakeDatabase.Users.Any(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            var user = new User
            {
                Id = FakeDatabase.GetNextUserId(),
                Email = model.Email,
                Password = model.Password ?? "123456", // Default password
                FullName = model.FullName,
                Role = model.Role,
                Status = model.Status,
                CreatedDate = DateTime.Now
            };

            FakeDatabase.Users.Add(user);
            return true;
        }

        public bool Update(UserFormViewModel model)
        {
            var user = GetById(model.Id ?? 0);
            if (user == null) return false;

            // Ki?m tra email trùng (ngo?i tr? user hi?n t?i)
            if (FakeDatabase.Users.Any(u => u.Id != user.Id && 
                u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            user.Email = model.Email;
            user.FullName = model.FullName;
            user.Role = model.Role;
            user.Status = model.Status;

            if (!string.IsNullOrEmpty(model.Password))
            {
                user.Password = model.Password;
            }

            return true;
        }

        public bool Delete(int id)
        {
            var user = GetById(id);
            if (user == null) return false;

            FakeDatabase.Users.Remove(user);
            return true;
        }

        public bool ChangeUserRole(int userId, UserRole newRole)
        {
            var user = GetById(userId);
            if (user == null) return false;

            user.Role = newRole;
            return true;
        }

        public bool ChangeUserStatus(int userId, UserStatus newStatus)
        {
            var user = GetById(userId);
            if (user == null) return false;

            user.Status = newStatus;
            return true;
        }

        public bool UpdateProfile(int userId, string fullName)
        {
            var user = GetById(userId);
            if (user == null) return false;

            user.FullName = fullName;
            return true;
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = GetById(userId);
            if (user == null) return false;

            if (user.Password != currentPassword)
            {
                return false;
            }

            user.Password = newPassword;
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
