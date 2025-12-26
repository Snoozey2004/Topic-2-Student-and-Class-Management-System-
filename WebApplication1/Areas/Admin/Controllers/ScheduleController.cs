using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Data;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ScheduleController : Controller
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        // GET: Admin/Schedule
        public IActionResult Index()
        {
            var schedules = _scheduleService.GetAll();
            return View(schedules);
        }

        // GET: Admin/Schedule/Details/5
        public IActionResult Details(int id)
        {
            var schedule = _scheduleService.GetById(id);
            if (schedule == null)
            {
                return NotFound();
            }

            var courseClass = FakeDatabase.CourseClasses.FirstOrDefault(c => c.Id == schedule.CourseClassId);
            var subject = courseClass != null 
                ? FakeDatabase.Subjects.FirstOrDefault(s => s.Id == courseClass.SubjectId) 
                : null;
            var lecturer = courseClass != null 
                ? FakeDatabase.Lecturers.FirstOrDefault(l => l.Id == courseClass.LecturerId) 
                : null;

            var model = new ScheduleDetailViewModel
            {
                Id = schedule.Id,
                ClassCode = courseClass?.ClassCode ?? "",
                SubjectName = subject?.SubjectName ?? "",
                LecturerName = lecturer?.FullName ?? "",
                DayOfWeek = schedule.DayOfWeek.ToString(),
                Session = schedule.Session,
                Period = schedule.Period,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Room = schedule.Room,
                EffectiveDate = schedule.EffectiveDate,
                EndDate = schedule.EndDate
            };

            return View(model);
        }

        // GET: Admin/Schedule/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            var model = new ScheduleFormViewModel
            {
                EffectiveDate = DateTime.Now
            };
            return View(model);
        }

        // POST: Admin/Schedule/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ScheduleFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = _scheduleService.Create(model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Schedule created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                TempData["ErrorMessage"] = "Schedule conflict detected! Please choose a different time or room.";
            }

            PopulateDropdowns();
            return View(model);
        }

        // GET: Admin/Schedule/Edit/5
        public IActionResult Edit(int id)
        {
            var schedule = _scheduleService.GetById(id);
            if (schedule == null)
            {
                return NotFound();
            }

            var model = new ScheduleFormViewModel
            {
                Id = schedule.Id,
                CourseClassId = schedule.CourseClassId,
                DayOfWeek = schedule.DayOfWeek,
                Session = schedule.Session,
                Period = schedule.Period,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Room = schedule.Room,
                EffectiveDate = schedule.EffectiveDate,
                EndDate = schedule.EndDate
            };

            PopulateDropdowns();
            return View(model);
        }

        // POST: Admin/Schedule/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ScheduleFormViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var success = _scheduleService.Update(model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Schedule updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                TempData["ErrorMessage"] = "Schedule conflict detected! Please choose a different time or room.";
            }

            PopulateDropdowns();
            return View(model);
        }

        // GET: Admin/Schedule/Delete/5
        public IActionResult Delete(int id)
        {
            var schedules = _scheduleService.GetAll();
            var schedule = schedules.FirstOrDefault(s => s.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Admin/Schedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var success = _scheduleService.Delete(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Schedule deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete schedule!";
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns()
        {
            var courseClasses = FakeDatabase.CourseClasses.Select(c =>
            {
                var subject = FakeDatabase.Subjects.FirstOrDefault(s => s.Id == c.SubjectId);
                return new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.ClassCode} - {subject?.SubjectName ?? ""}"
                };
            }).ToList();

            var days = Enum.GetValues<DayOfWeek>()
                .Where(d => d != DayOfWeek.Sunday && d != DayOfWeek.Saturday)
                .Select(d => new SelectListItem
                {
                    Value = ((int)d).ToString(),
                    Text = d.ToString()
                }).ToList();

            var sessions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Morning", Text = "Morning" },
                new SelectListItem { Value = "Afternoon", Text = "Afternoon" },
                new SelectListItem { Value = "Evening", Text = "Evening" }
            };

            var periods = new List<SelectListItem>
            {
                new SelectListItem { Value = "Period 1-3", Text = "Period 1-3 (07:00-09:30)" },
                new SelectListItem { Value = "Period 4-6", Text = "Period 4-6 (09:45-12:15)" },
                new SelectListItem { Value = "Period 7-9", Text = "Period 7-9 (13:00-15:30)" }
            };

            var rooms = new List<SelectListItem>
            {
                new SelectListItem { Value = "A101", Text = "A101" },
                new SelectListItem { Value = "A102", Text = "A102" },
                new SelectListItem { Value = "A103", Text = "A103" },
                new SelectListItem { Value = "B201", Text = "B201" },
                new SelectListItem { Value = "B202", Text = "B202" },
                new SelectListItem { Value = "C301", Text = "C301" },
                new SelectListItem { Value = "C302", Text = "C302" },
                new SelectListItem { Value = "Lab301", Text = "Lab301" },
                new SelectListItem { Value = "Lab302", Text = "Lab302" }
            };

            ViewBag.CourseClasses = courseClasses;
            ViewBag.Days = days;
            ViewBag.Sessions = sessions;
            ViewBag.Periods = periods;
            ViewBag.Rooms = rooms;
        }
    }
}
