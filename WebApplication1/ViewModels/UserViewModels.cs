using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    /// <summary>
    /// ViewModel cho t?o/ch?nh s?a User
    /// </summary>
    public class UserFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Email là b?t bu?c")]
        [EmailAddress(ErrorMessage = "Email không h?p l?")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "H? và tên là b?t bu?c")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vai trò là b?t bu?c")]
        public UserRole Role { get; set; }

        [Required(ErrorMessage = "Tr?ng thái là b?t bu?c")]
        public UserStatus Status { get; set; }

        // Password ch? required khi t?o m?i
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }

    /// <summary>
    /// ViewModel cho danh sách User
    /// </summary>
    public class UserListViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
