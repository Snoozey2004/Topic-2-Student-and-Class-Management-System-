using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Admin/User
        public IActionResult Index()
        {
            var users = _userService.GetAll();
            return View(users);
        }

        // GET: Admin/User/Details/5
        public IActionResult Details(int id)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // GET: Admin/User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(WebApplication1.ViewModels.UserFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = _userService.Create(model);
            if (!success)
            {
                ModelState.AddModelError("", "Email already exists");
                return View(model);
            }

            TempData["SuccessMessage"] = "User created successfully";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/User/Edit/5
        public IActionResult Edit(int id)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound();

            var model = new WebApplication1.ViewModels.UserFormViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Status = user.Status
            };

            return View(model);
        }

        // POST: Admin/User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(WebApplication1.ViewModels.UserFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = _userService.Update(model);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to update user");
                return View(model);
            }

            TempData["SuccessMessage"] = "User updated successfully";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/User/Delete/5
        public IActionResult Delete(int id)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: Admin/User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var success = _userService.Delete(id);
            if (!success)
            {
                TempData["ErrorMessage"] = "Unable to delete user";
            }
            else
            {
                TempData["SuccessMessage"] = "User deleted successfully";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
