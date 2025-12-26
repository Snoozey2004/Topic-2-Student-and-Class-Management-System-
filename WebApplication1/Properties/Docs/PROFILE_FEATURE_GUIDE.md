# ?? HÝ?NG D?N: T?O CH?C NÃNG PROFILE VÀ CHANGE PASSWORD

## ?? LÝU ?

Do có xung ð?t v?i code hi?n t?i (ChangePasswordViewModel ð? t?n t?i và UserService.Update signature khác), tôi ð? remove các file ð? t?o.

B?n c?n th?c hi?n các bý?c sau ð? t?o ch?c nãng Profile và Change Password:

---

## ?? BÝ?C 1: Ki?m tra AuthViewModels.cs

Ki?m tra xem file `ViewModels/AuthViewModels.cs` ð? có `ChangePasswordViewModel` chýa.

N?u **CHÝA CÓ**, thêm vào cu?i file:

```csharp
public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Current password is required")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ProfileViewModel
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    
    // For Students
    public string? StudentCode { get; set; }
    public string? Major { get; set; }
    public int? AdmissionYear { get; set; }
    
    // For Lecturers
    public string? LecturerCode { get; set; }
    public string? Department { get; set; }
    public string? Specialization { get; set; }
}
```

---

## ?? BÝ?C 2: T?o ProfileController

T?o file `Controllers/ProfileController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using WebApplication1.ViewModels;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly ILecturerService _lecturerService;

        public ProfileController(
            IAuthService authService,
            IUserService userService,
            IStudentService studentService,
            ILecturerService lecturerService)
        {
            _authService = authService;
            _userService = userService;
            _studentService = studentService;
            _lecturerService = lecturerService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);
            var user = _userService.GetById(userId);
            
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                AccountId = $"ADMIN{user.Id:D3}",
                Role = user.Role.ToString()
            };

            // Load additional info based on role
            if (user.Role == UserRole.Student)
            {
                var student = _studentService.GetByUserId(userId);
                if (student != null)
                {
                    model.StudentCode = student.StudentCode;
                    model.Major = student.Major;
                    model.AdmissionYear = student.AdmissionYear;
                    model.AccountId = student.StudentCode;
                }
            }
            else if (user.Role == UserRole.Lecturer)
            {
                var lecturer = _lecturerService.GetByUserId(userId);
                if (lecturer != null)
                {
                    model.LecturerCode = lecturer.LecturerCode;
                    model.Department = lecturer.Department;
                    model.Specialization = lecturer.Specialization;
                    model.AccountId = lecturer.LecturerCode;
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);
            var user = _userService.GetById(userId);
            
            if (user == null)
            {
                return NotFound();
            }

            // Verify current password
            if (user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect");
                return View(model);
            }

            // Update password directly in FakeDatabase
            user.Password = model.NewPassword;
            // Note: UserService.Update có th? không có, b?n c?n update tr?c ti?p vào FakeDatabase.Users

            TempData["SuccessMessage"] = "Password changed successfully";
            return RedirectToAction("Index");
        }
    }
}
```

---

## ?? BÝ?C 3: T?o Views

### 3.1. Profile Index View

T?o thý m?c `Views/Profile/` và file `Index.cshtml`:

(Copy n?i dung t? file design m?u - xem ?nh b?n g?i)

### 3.2. Change Password View

T?o file `Views/Profile/ChangePassword.cshtml`:

(Copy n?i dung t? file design m?u - xem ?nh b?n g?i)

---

## ?? BÝ?C 4: Update _DashboardLayout.cshtml

Trong user dropdown menu, thêm 2 links:

```html
<ul class="dropdown-menu dropdown-menu-end">
    <li><a class="dropdown-item" asp-area="" asp-controller="Profile" asp-action="Index">
        <i class="bi bi-person me-2"></i>Profile
    </a></li>
    <li><a class="dropdown-item" asp-area="" asp-controller="Profile" asp-action="ChangePassword">
        <i class="bi bi-key me-2"></i>Change Password
    </a></li>
    <li><hr class="dropdown-divider"></li>
    <li>
        <form asp-area="" asp-controller="Account" asp-action="Logout" method="post">
            <button type="submit" class="dropdown-item">
                <i class="bi bi-box-arrow-right me-2"></i>Logout
            </button>
        </form>
    </li>
</ul>
```

---

## ? K?T QU?

Sau khi hoàn thành, b?n s? có:

1. **Profile Page** (`/Profile/Index`):
   - Hi?n th? thông tin cá nhân
   - Avatar v?i ch? cái ð?u
   - Full Name, Email, Account ID, Role
   - Thông tin b? sung (Student: Major, Year; Lecturer: Department)

2. **Change Password Page** (`/Profile/ChangePassword`):
   - Form ð?i m?t kh?u
   - Current Password
   - New Password  
   - Confirm New Password
   - Toggle show/hide password

---

## ?? DESIGN FEATURES

### Profile Page:
- Avatar tr?n v?i gradient Navy Blue
- Card tr?ng v?i shadow
- Info fields v?i icons
- Navy Blue theme consistent

### Change Password Page:
- Form centered (max-width 600px)
- Password toggle buttons (eye icon)
- Password requirements list
- Navy Blue submit button

---

## ?? TEST

1. Login v?i b?t k? account nào
2. Click vào avatar ? góc ph?i trên
3. Ch?n "Profile" ? Xem thông tin
4. Ch?n "Change Password" ? Ð?i m?t kh?u
5. Test ð?i m?t kh?u và login l?i

---

**N?u b?n g?p l?i, vui l?ng ki?m tra:**
- ChangePasswordViewModel ð? có trong AuthViewModels.cs chýa
- UserService có method Update không
- FakeDatabase có cho phép update User.Password không

Tôi s?n sàng h? tr? n?u b?n c?n thêm chi ti?t!
