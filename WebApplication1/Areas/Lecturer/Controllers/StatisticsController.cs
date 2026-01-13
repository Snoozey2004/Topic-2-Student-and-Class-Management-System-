using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using ClosedXML.Excel;

namespace WebApplication1.Areas.Lecturer.Controllers
{
    [Area("Lecturer")]
    public class StatisticsController : Controller
    {
        private readonly ILecturerService _lecturerService;
        private readonly ILecturerStatisticsService _statisticsService;

        public StatisticsController(
            ILecturerService lecturerService,
            ILecturerStatisticsService statisticsService)
        {
            _lecturerService = lecturerService;
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
            var lecturer = _lecturerService.GetByUserId(userId);
            if (lecturer == null)
            {
                return NotFound();
            }

            var model = _statisticsService.GetStatistics(lecturer.Id);
            ViewData["Title"] = "Teaching Statistics";
            ViewData["PageTitle"] = "Teaching Statistics";

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
            var lecturer = _lecturerService.GetByUserId(userId);
            if (lecturer == null)
            {
                return NotFound();
            }

            var model = _statisticsService.GetStatistics(lecturer.Id);

            using var workbook = new XLWorkbook();

            var overview = workbook.Worksheets.Add("Overview");
            overview.Cell(1, 1).Value = "Lecturer";
            overview.Cell(1, 2).Value = lecturer.FullName;
            overview.Cell(2, 1).Value = "Classes";
            overview.Cell(2, 2).Value = model.TotalClasses;
            overview.Cell(3, 1).Value = "Students";
            overview.Cell(3, 2).Value = model.TotalStudents;

            var gradeSheet = workbook.Worksheets.Add("Grades");
            gradeSheet.Cell(1, 1).Value = "Class";
            gradeSheet.Cell(1, 2).Value = "Subject";
            gradeSheet.Cell(1, 3).Value = "Avg";
            gradeSheet.Cell(1, 4).Value = "Excellent";
            gradeSheet.Cell(1, 5).Value = "Good";
            gradeSheet.Cell(1, 6).Value = "Fair";
            gradeSheet.Cell(1, 7).Value = "Average";
            gradeSheet.Cell(1, 8).Value = "Weak";
            var row = 2;
            foreach (var g in model.GradeStats)
            {
                gradeSheet.Cell(row, 1).Value = g.ClassCode;
                gradeSheet.Cell(row, 2).Value = g.SubjectName;
                gradeSheet.Cell(row, 3).Value = g.AverageScore;
                gradeSheet.Cell(row, 4).Value = g.ExcellentCount;
                gradeSheet.Cell(row, 5).Value = g.GoodCount;
                gradeSheet.Cell(row, 6).Value = g.FairCount;
                gradeSheet.Cell(row, 7).Value = g.AverageCount;
                gradeSheet.Cell(row, 8).Value = g.WeakCount;
                row++;
            }

            if (model.StudentsWithoutScores.Any())
            {
                gradeSheet.Cell(row + 1, 1).Value = "Students without scores";
                gradeSheet.Cell(row + 2, 1).Value = "Code";
                gradeSheet.Cell(row + 2, 2).Value = "Name";
                gradeSheet.Cell(row + 2, 3).Value = "Class";
                gradeSheet.Cell(row + 2, 4).Value = "Subject";
                var nsRow = row + 3;
                foreach (var s in model.StudentsWithoutScores)
                {
                    gradeSheet.Cell(nsRow, 1).Value = s.StudentCode;
                    gradeSheet.Cell(nsRow, 2).Value = s.StudentName;
                    gradeSheet.Cell(nsRow, 3).Value = s.ClassCode;
                    gradeSheet.Cell(nsRow, 4).Value = s.SubjectName;
                    nsRow++;
                }
            }

            var attendanceSheet = workbook.Worksheets.Add("Attendance");
            attendanceSheet.Cell(1, 1).Value = "Class";
            attendanceSheet.Cell(1, 2).Value = "Subject";
            attendanceSheet.Cell(1, 3).Value = "Avg %";
            attendanceSheet.Cell(1, 4).Value = "High absence";
            var aRow = 2;
            foreach (var a in model.AttendanceStats)
            {
                attendanceSheet.Cell(aRow, 1).Value = a.ClassCode;
                attendanceSheet.Cell(aRow, 2).Value = a.SubjectName;
                attendanceSheet.Cell(aRow, 3).Value = a.AverageAttendanceRate;
                attendanceSheet.Cell(aRow, 4).Value = a.HighAbsenceCount;
                aRow++;
            }

            attendanceSheet.Cell(aRow + 1, 1).Value = "Absence alerts";
            attendanceSheet.Cell(aRow + 2, 1).Value = "Student";
            attendanceSheet.Cell(aRow + 2, 2).Value = "Rate";
            attendanceSheet.Cell(aRow + 2, 3).Value = "Class";
            var alertRow = aRow + 3;
            foreach (var a in model.AttendanceStats)
            {
                foreach (var s in a.HighAbsenceStudents)
                {
                    attendanceSheet.Cell(alertRow, 1).Value = $"{s.StudentCode} - {s.StudentName}";
                    attendanceSheet.Cell(alertRow, 2).Value = s.AttendanceRate;
                    attendanceSheet.Cell(alertRow, 3).Value = a.ClassCode;
                    alertRow++;
                }
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TeachingStatistics.xlsx");
        }
    }
}
