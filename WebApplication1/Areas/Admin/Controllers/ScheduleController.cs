using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ScheduleController : Controller
    {
        private readonly IScheduleService _scheduleService;
        private readonly ApplicationDbContext _db;

        public ScheduleController(IScheduleService scheduleService, ApplicationDbContext db)
        {
            _scheduleService = scheduleService;
            _db = db;
        }

        // GET: Admin/Schedule
        public IActionResult Index()
        {
            var schedules = _scheduleService.GetAll();
            return View(schedules);
        }

        // GET: Admin/Schedule/Export
        public IActionResult Export()
        {
            // Build export from DB so we can include columns needed for import
            var data = (
                from s in _db.Schedules.AsNoTracking()
                join cc in _db.CourseClasses.AsNoTracking() on s.CourseClassId equals cc.Id
                select new
                {
                    cc.ClassCode,
                    s.DayOfWeek,
                    s.Session,
                    s.Period,
                    s.StartTime,
                    s.EndTime,
                    s.Room,
                    s.EffectiveDate,
                    s.EndDate
                }
            ).ToList();

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Schedules");

            var headers = new[]
            {
                "ClassCode",
                "DayOfWeek",
                "Session",
                "Period",
                "StartTime",
                "EndTime",
                "Room",
                "EffectiveDate",
                "EndDate"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
                ws.Cell(1, i + 1).Style.Font.Bold = true;
            }

            var row = 2;
            foreach (var x in data)
            {
                ws.Cell(row, 1).Value = x.ClassCode;
                ws.Cell(row, 2).Value = x.DayOfWeek.ToString();
                ws.Cell(row, 3).Value = x.Session;
                ws.Cell(row, 4).Value = x.Period;
                ws.Cell(row, 5).Value = x.StartTime;
                ws.Cell(row, 6).Value = x.EndTime;
                ws.Cell(row, 7).Value = x.Room;

                ws.Cell(row, 8).Value = x.EffectiveDate.Date;
                ws.Cell(row, 8).Style.DateFormat.Format = "yyyy-MM-dd";

                if (x.EndDate.HasValue)
                {
                    ws.Cell(row, 9).Value = x.EndDate.Value.Date;
                    ws.Cell(row, 9).Style.DateFormat.Format = "yyyy-MM-dd";
                }

                row++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            ms.Position = 0;

            var fileName = $"schedules-{DateTime.Now:yyyyMMdd-HHmmss}.xlsx";
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // GET: Admin/Schedule/Import
        public IActionResult Import()
        {
            return View();
        }

        // POST: Admin/Schedule/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select an Excel file.";
                return View();
            }

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Invalid file type. Please upload a .xlsx file.";
                return View();
            }

            var classCodeToId = _db.CourseClasses
                .AsNoTracking()
                .ToDictionary(c => c.ClassCode, c => c.Id, StringComparer.OrdinalIgnoreCase);

            var imported = 0;
            var skipped = 0;
            var invalid = 0;
            var errors = new List<string>();

            try
            {
                using var stream = file.OpenReadStream();
                var rows = ScheduleExcelImport.Parse(stream, classCodeToId);

                foreach (var r in rows)
                {
                    if (r.RowNumber == 0)
                    {
                        errors.Add(r.Error ?? "Invalid Excel file");
                        continue;
                    }

                    if (r.Model == null)
                    {
                        invalid++;
                        errors.Add($"Row {r.RowNumber}: {r.Error}");
                        continue;
                    }

                    var ok = _scheduleService.Create(r.Model);
                    if (ok) imported++;
                    else
                    {
                        skipped++;
                        errors.Add($"Row {r.RowNumber}: conflict detected (schedule not imported)");
                    }
                }

                ViewBag.ImportSummary = $"Imported: {imported}. Skipped (conflicts): {skipped}. Invalid: {invalid}.";
                if (errors.Count > 0)
                    ViewBag.ImportErrors = errors;

                if (imported > 0)
                    TempData["SuccessMessage"] = $"Imported {imported} schedules.";
                if (imported == 0 && errors.Count == 0)
                    TempData["ErrorMessage"] = "No schedules were imported.";

                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to import Excel: {ex.Message}";
                return View();
            }
        }

        // GET: Admin/Schedule/Details/5
        public IActionResult Details(int id)
        {
            var schedule = _scheduleService.GetById(id);
            if (schedule == null)
                return NotFound();

            // Lấy info class + subject + lecturer từ DB
            var data =
                (from c in _db.CourseClasses.AsNoTracking()
                 join s in _db.Subjects.AsNoTracking() on c.SubjectId equals s.Id into sj
                 from subject in sj.DefaultIfEmpty()
                 join l in _db.Lecturers.AsNoTracking() on c.LecturerId equals l.Id into lj
                 from lecturer in lj.DefaultIfEmpty()
                 where c.Id == schedule.CourseClassId
                 select new
                 {
                     ClassCode = c.ClassCode,
                     SubjectName = subject != null ? subject.SubjectName : "",
                     LecturerName = lecturer != null ? lecturer.FullName : ""
                 })
                .FirstOrDefault();

            var model = new ScheduleDetailViewModel
            {
                Id = schedule.Id,
                ClassCode = data?.ClassCode ?? "",
                SubjectName = data?.SubjectName ?? "",
                LecturerName = data?.LecturerName ?? "",
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
                return NotFound();

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
                return NotFound();

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
            var schedule = _scheduleService.GetById(id);
            if (schedule == null)
                return NotFound();

            // nếu View Delete đang nhận ScheduleListViewModel thì giữ cách cũ.
            // còn nếu View nhận Schedule entity thì trả thẳng schedule cũng được.
            // Ở đây giữ giống flow cũ: lấy từ list service (nhưng không load all).
            var item = _scheduleService.GetAll().FirstOrDefault(s => s.Id == id);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: Admin/Schedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var success = _scheduleService.Delete(id);
            if (success)
                TempData["SuccessMessage"] = "Schedule deleted successfully!";
            else
                TempData["ErrorMessage"] = "Failed to delete schedule!";

            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns()
        {
            // CourseClasses dropdown (kèm SubjectName)
            var courseClasses =
                (from c in _db.CourseClasses.AsNoTracking()
                 join s in _db.Subjects.AsNoTracking() on c.SubjectId equals s.Id into sj
                 from subject in sj.DefaultIfEmpty()
                 orderby c.Semester descending, c.ClassCode
                 select new SelectListItem
                 {
                     Value = c.Id.ToString(),
                     Text = $"{c.ClassCode} - {(subject != null ? subject.SubjectName : "")}"
                 })
                .ToList();

            var days = Enum.GetValues<DayOfWeek>()
                .Where(d => d != DayOfWeek.Sunday && d != DayOfWeek.Saturday)
                .Select(d => new SelectListItem
                {
                    Value = ((int)d).ToString(),
                    Text = d.ToString()
                })
                .ToList();

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
