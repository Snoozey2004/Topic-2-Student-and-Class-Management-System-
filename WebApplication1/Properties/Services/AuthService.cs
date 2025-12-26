using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service x? lý authentication và authorization
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
        public User? Authenticate(string email, string password)
        {
            var user = FakeDatabase.Users.FirstOrDefault(u => 
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && 
                u.Password == password);

            if (user != null && user.Status == UserStatus.Active)
            {
                user.LastLoginDate = DateTime.Now;
                return user;
            }

            return null;
        }

        public User? GetUserByEmail(string email)
        {
            return FakeDatabase.Users.FirstOrDefault(u => 
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public User? GetUserById(int id)
        {
            return FakeDatabase.Users.FirstOrDefault(u => u.Id == id);
        }

        public bool Register(string email, string password, string fullName)
        {
            // Ki?m tra email ?ã t?n t?i ch?a
            if (GetUserByEmail(email) != null)
            {
                return false;
            }

            var user = new User
            {
                Id = FakeDatabase.GetNextUserId(),
                Email = email,
                Password = password,
                FullName = fullName,
                Role = UserRole.Student,
                Status = UserStatus.Pending, // Ch? admin duy?t
                CreatedDate = DateTime.Now
            };

            FakeDatabase.Users.Add(user);

            // T?o b?n ghi Student
            var student = new Student
            {
                Id = FakeDatabase.GetNextStudentId(),
                UserId = user.Id,
                StudentCode = $"SV{DateTime.Now:yyyyMMddHHmmss}",
                FullName = fullName,
                Email = email,
                DateOfBirth = DateTime.Now.AddYears(-20),
                PhoneNumber = "",
                Address = "",
                Major = "Ch?a xác ??nh",
                AdmissionYear = DateTime.Now.Year,
                CreatedDate = DateTime.Now
            };

            FakeDatabase.Students.Add(student);

            return true;
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = GetUserById(userId);
            if (user == null || user.Password != currentPassword)
            {
                return false;
            }

            user.Password = newPassword;
            return true;
        }

        public bool ResetPassword(string email, string newPassword)
        {
            var user = GetUserByEmail(email);
            if (user == null)
            {
                return false;
            }

            user.Password = newPassword;
            return true;
        }
    }
}
