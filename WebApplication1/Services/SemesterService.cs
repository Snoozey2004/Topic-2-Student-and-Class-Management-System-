using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service quản lý Semester - Tập trung logic semester để dễ thay đổi khi connect DB thật
    /// </summary>
    public interface ISemesterService
    {
        string GetCurrentSemester();
        List<string> GetAllSemesters();
        bool IsEnrollmentOpen(string semester);
        string GetDefaultSemester();
    }

    public class SemesterService : ISemesterService
    {
        private readonly ApplicationDbContext _db;

        public SemesterService(ApplicationDbContext db)
        {
            _db = db;
        }

        public string GetCurrentSemester()
        {
            // Lấy semester từ các lớp đang Open hoặc InProgress
            var currentSemester = _db.CourseClasses
                .AsNoTracking()
                .Where(c => c.Status == CourseClassStatus.Open || c.Status == CourseClassStatus.InProgress)
                .OrderByDescending(c => c.Semester)
                .Select(c => c.Semester)
                .FirstOrDefault();

            // Fallback nếu không tìm thấy
            if (string.IsNullOrWhiteSpace(currentSemester))
            {
                currentSemester = _db.CourseClasses
                    .AsNoTracking()
                    .OrderByDescending(c => c.Semester)
                    .Select(c => c.Semester)
                    .FirstOrDefault() ?? "HK1-2024";
            }

            return currentSemester;
        }

        public List<string> GetAllSemesters()
        {
            return _db.CourseClasses
                .AsNoTracking()
                .Select(c => c.Semester)
                .Distinct()
                .OrderByDescending(s => s)
                .ToList();
        }

        public bool IsEnrollmentOpen(string semester)
        {
            return _db.CourseClasses
                .AsNoTracking()
                .Any(c => c.Semester == semester && c.Status == CourseClassStatus.Open);
        }

        public string GetDefaultSemester()
        {
            return GetCurrentSemester();
        }
    }
}
