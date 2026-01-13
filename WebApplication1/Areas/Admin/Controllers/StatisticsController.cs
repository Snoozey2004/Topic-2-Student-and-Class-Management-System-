using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using ClosedXML.Excel;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StatisticsController : Controller
    {
        private readonly IAdminStatisticsService _statisticsService;

        public StatisticsController(IAdminStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public IActionResult Index()
        {
            var model = _statisticsService.GetStatistics();
            ViewData["Title"] = "Overall Statistics";
            ViewData["PageTitle"] = "Overall Statistics";
            return View(model);
        }

        [HttpGet]
        public IActionResult Export()
        {
            var model = _statisticsService.GetStatistics();

            using var workbook = new XLWorkbook();

            var overview = workbook.Worksheets.Add("Overview");
            overview.Cell(1, 1).Value = "Total Students";
            overview.Cell(1, 2).Value = model.TotalStudents;
            overview.Cell(2, 1).Value = "Total Lecturers";
            overview.Cell(2, 2).Value = model.TotalLecturers;
            overview.Cell(3, 1).Value = "Total Classes";
            overview.Cell(3, 2).Value = model.TotalClasses;

            var scoreSheet = workbook.Worksheets.Add("Scores");
            scoreSheet.Cell(1, 1).Value = "Average Score";
            scoreSheet.Cell(1, 2).Value = model.AverageScore;
            scoreSheet.Cell(2, 1).Value = "Pass Rate (%)";
            scoreSheet.Cell(2, 2).Value = model.PassRate;
            scoreSheet.Cell(3, 1).Value = "Low Score Warnings";
            scoreSheet.Cell(3, 2).Value = model.WarningLowScoreCount;

            scoreSheet.Cell(5, 1).Value = "Score Distribution";
            scoreSheet.Cell(6, 1).Value = "Range";
            scoreSheet.Cell(6, 2).Value = "Count";
            var row = 7;
            foreach (var item in model.ScoreDistribution)
            {
                scoreSheet.Cell(row, 1).Value = item.Label;
                scoreSheet.Cell(row, 2).Value = item.Count;
                row++;
            }

            scoreSheet.Cell(row + 1, 1).Value = "Top Students";
            scoreSheet.Cell(row + 2, 1).Value = "Code";
            scoreSheet.Cell(row + 2, 2).Value = "Name";
            scoreSheet.Cell(row + 2, 3).Value = "GPA";
            var topRow = row + 3;
            foreach (var s in model.TopStudents)
            {
                scoreSheet.Cell(topRow, 1).Value = s.StudentCode;
                scoreSheet.Cell(topRow, 2).Value = s.StudentName;
                scoreSheet.Cell(topRow, 3).Value = s.GPA;
                topRow++;
            }

            scoreSheet.Cell(topRow + 1, 1).Value = "Score Trend";
            scoreSheet.Cell(topRow + 2, 1).Value = "Semester";
            scoreSheet.Cell(topRow + 2, 2).Value = "Avg";
            var trendRow = topRow + 3;
            foreach (var t in model.ScoreTrendBySemester)
            {
                scoreSheet.Cell(trendRow, 1).Value = t.Semester;
                scoreSheet.Cell(trendRow, 2).Value = t.Value;
                trendRow++;
            }

            var attendanceSheet = workbook.Worksheets.Add("Attendance");
            attendanceSheet.Cell(1, 1).Value = "Average Attendance (%)";
            attendanceSheet.Cell(1, 2).Value = model.AverageAttendanceRate;
            attendanceSheet.Cell(2, 1).Value = "High Absence Students";
            attendanceSheet.Cell(2, 2).Value = model.HighAbsenceStudentCount;

            attendanceSheet.Cell(4, 1).Value = "By Class";
            attendanceSheet.Cell(5, 1).Value = "Class";
            attendanceSheet.Cell(5, 2).Value = "Subject";
            attendanceSheet.Cell(5, 3).Value = "Attendance %";
            attendanceSheet.Cell(5, 4).Value = "Students";
            var attRow = 6;
            foreach (var c in model.AttendanceByClass)
            {
                attendanceSheet.Cell(attRow, 1).Value = c.ClassCode;
                attendanceSheet.Cell(attRow, 2).Value = c.SubjectName;
                attendanceSheet.Cell(attRow, 3).Value = c.AttendanceRate;
                attendanceSheet.Cell(attRow, 4).Value = c.StudentCount;
                attRow++;
            }

            attendanceSheet.Cell(attRow + 1, 1).Value = "Attendance Trend";
            attendanceSheet.Cell(attRow + 2, 1).Value = "Semester";
            attendanceSheet.Cell(attRow + 2, 2).Value = "Rate";
            var attTrendRow = attRow + 3;
            foreach (var t in model.AttendanceTrendBySemester)
            {
                attendanceSheet.Cell(attTrendRow, 1).Value = t.Semester;
                attendanceSheet.Cell(attTrendRow, 2).Value = t.Value;
                attTrendRow++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OverallStatistics.xlsx");
        }
    }
}
