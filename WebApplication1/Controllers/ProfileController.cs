using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using WebApplication1.ViewModels;
using WebApplication1.Models;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly ILecturerService _lecturerService;

        public ProfileController(
            IUserService userService,
            IStudentService studentService,
            ILecturerService lecturerService)
        {
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
                AccountId = $"USER{user.Id:D3}",
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

        [HttpPost]
        public IActionResult Update(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fix the errors";
                return RedirectToAction("Index");
            }

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);
            var user = FakeDatabase.Users.FirstOrDefault(u => u.Id == userId);
            
            if (user == null)
            {
                return NotFound();
            }

            // Update full name
            user.FullName = model.FullName;

            TempData["SuccessMessage"] = "Profile updated successfully";
            return RedirectToAction("Index");
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
            var user = FakeDatabase.Users.FirstOrDefault(u => u.Id == userId);
            
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

            // Update password
            user.Password = model.NewPassword;

            TempData["SuccessMessage"] = "Password changed successfully";
            return RedirectToAction("Index");
        }
    }
}
