using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// Controller cho trang ch? công khai (ch?a ??ng nh?p)
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Trang ch? - ch? hi?n th? 2 card: ??ng nh?p và ??ng ký
        /// </summary>
        public IActionResult Index()
        {
            // N?u ?ã ??ng nh?p thì redirect v? dashboard t??ng ?ng
            if (User.Identity?.IsAuthenticated == true)
            {
                var roleClaim = User.FindFirst("Role");
                if (roleClaim != null)
                {
                    var role = Enum.Parse<UserRole>(roleClaim.Value);
                    return role switch
                    {
                        UserRole.Admin => Redirect("/Admin/Dashboard"),
                        UserRole.Lecturer => Redirect("/Lecturer/Dashboard"),
                        UserRole.Student => Redirect("/Student/Dashboard"),
                        _ => View()
                    };
                }
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
