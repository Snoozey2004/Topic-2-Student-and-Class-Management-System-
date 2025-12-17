using Microsoft.AspNetCore.Mvc;
using WebApplication1.ViewModels;
using WebApplication1.Services;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GradeController : Controller
    {
        private readonly IGradeService _gradeService;

        public GradeController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        public IActionResult Index()
        {
            var grades = _gradeService.GetAll(); // Updated to use new GetAll method
            return View(grades);
        }
    }
}
