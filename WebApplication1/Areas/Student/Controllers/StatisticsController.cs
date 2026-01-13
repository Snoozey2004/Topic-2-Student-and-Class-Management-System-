using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using ClosedXML.Excel;
using System.IO;

namespace WebApplication1.Areas.Student.Controllers
{
    [Area("Student")]
    public class StatisticsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IStudentStatisticsService _statisticsService;

        public StatisticsController(
            IStudentService studentService,
            IStudentStatisticsService statisticsService)
        {
            _studentService = studentService;
            _statisticsService = statisticsService;
        }

        public IActionResult Index()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var userId = int.Parse(userIdClaim.Value);
            var student = _studentService.GetByUserId(userId);
            if (student == null)
            {
                return NotFound();
            }

            var model = _statisticsService.GetStatistics(student.Id);
            ViewData["Title"] = "Personal Statistics";
            ViewData["PageTitle"] = "Personal Statistics";

            return View(model);
        }

        [HttpGet]
        public IActionResult Export()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var userId = int.Parse(userIdClaim.Value);
            var student = _studentService.GetByUserId(userId);
            if (student == null)
            {
                return NotFound();
            }

            var model = _statisticsService.GetStatistics(student.Id);

            using var workbook = new XLWorkbook();

            var overview = workbook.Worksheets.Add("Overview");
            overview.Cell(1, 1).Value = "Student";
            overview.Cell(1, 2).Value = student.FullName;
            overview.Cell(2, 1).Value = "Student Code";
            overview.Cell(2, 2).Value = student.StudentCode;
            overview.Cell(3, 1).Value = "Courses";
            overview.Cell(3, 2).Value = model.CurrentCourseCount;
            overview.Cell(4, 1).Value = "Credits";
            overview.Cell(4, 2).Value = model.TotalRegisteredCredits;
            overview.Cell(5, 1).Value = "GPA";
            overview.Cell(5, 2).Value = model.GPA;

            var schedule = workbook.Worksheets.Add("Schedule");
            schedule.Cell(1, 1).Value = "Date";
            schedule.Cell(1, 2).Value = "Session";
            schedule.Cell(1, 3).Value = "Class";
            schedule.Cell(1, 4).Value = "Subject";
            schedule.Cell(1, 5).Value = "Room";
            var sRow = 2;
            foreach (var s in model.ThisWeekSchedule)
            {
                schedule.Cell(sRow, 1).Value = s.Date;
                schedule.Cell(sRow, 2).Value = s.Session;
                schedule.Cell(sRow, 3).Value = s.ClassCode;
                schedule.Cell(sRow, 4).Value = s.SubjectName;
                schedule.Cell(sRow, 5).Value = s.Room;
                sRow++;
            }

            var grades = workbook.Worksheets.Add("Grades");
            grades.Cell(1, 1).Value = "Class";
            grades.Cell(1, 2).Value = "Subject";
            grades.Cell(1, 3).Value = "Mid";
            grades.Cell(1, 4).Value = "Final";
            grades.Cell(1, 5).Value = "Total";
            grades.Cell(1, 6).Value = "Letter";
            grades.Cell(1, 7).Value = "Classification";
            var gRow = 2;
            foreach (var g in model.CourseGrades)
            {
                grades.Cell(gRow, 1).Value = g.ClassCode;
                grades.Cell(gRow, 2).Value = g.SubjectName;
                grades.Cell(gRow, 3).Value = g.MidtermScore;
                grades.Cell(gRow, 4).Value = g.FinalScore;
                grades.Cell(gRow, 5).Value = g.TotalScore;
                grades.Cell(gRow, 6).Value = g.LetterGrade;
                grades.Cell(gRow, 7).Value = g.Classification;
                gRow++;
            }

            var attendance = workbook.Worksheets.Add("Attendance");
            attendance.Cell(1, 1).Value = "Class";
            attendance.Cell(1, 2).Value = "Subject";
            attendance.Cell(1, 3).Value = "Present";
            attendance.Cell(1, 4).Value = "Total";
            attendance.Cell(1, 5).Value = "Rate";
            var aRow = 2;
            foreach (var c in model.AttendanceSummary.Courses)
            {
                attendance.Cell(aRow, 1).Value = c.ClassCode;
                attendance.Cell(aRow, 2).Value = c.SubjectName;
                attendance.Cell(aRow, 3).Value = c.PresentSessions;
                attendance.Cell(aRow, 4).Value = c.TotalSessions;
                attendance.Cell(aRow, 5).Value = c.AttendanceRate;
                aRow++;
            }

            attendance.Cell(aRow + 1, 1).Value = "Absent days";
            attendance.Cell(aRow + 2, 1).Value = "Date";
            attendance.Cell(aRow + 2, 2).Value = "Session";
            attendance.Cell(aRow + 2, 3).Value = "Class";
            attendance.Cell(aRow + 2, 4).Value = "Subject";
            attendance.Cell(aRow + 2, 5).Value = "Note";
            var adRow = aRow + 3;
            foreach (var ad in model.AttendanceSummary.AbsentDays)
            {
                attendance.Cell(adRow, 1).Value = ad.Date;
                attendance.Cell(adRow, 2).Value = ad.Session;
                attendance.Cell(adRow, 3).Value = ad.ClassCode;
                attendance.Cell(adRow, 4).Value = ad.SubjectName;
                attendance.Cell(adRow, 5).Value = ad.Note;
                adRow++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PersonalStatistics.xlsx");
        }
    }
}
