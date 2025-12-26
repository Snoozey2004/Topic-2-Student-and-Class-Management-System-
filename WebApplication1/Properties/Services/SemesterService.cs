using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service qu?n lý Semester - T?p trung logic semester ?? d? thay ??i khi connect DB th?t
    /// </summary>
    public interface ISemesterService
    {
        /// <summary>
        /// L?y semester hi?n t?i (?ang m? ??ng ký ho?c ?ang h?c)
        /// </summary>
        string GetCurrentSemester();

        /// <summary>
        /// L?y danh sách t?t c? semester có trong h? th?ng
        /// </summary>
        List<string> GetAllSemesters();

        /// <summary>
        /// Ki?m tra semester có ?ang trong th?i gian ??ng ký không
        /// </summary>
        bool IsEnrollmentOpen(string semester);

        /// <summary>
        /// L?y semester m?c ??nh cho các dropdown
        /// </summary>
        string GetDefaultSemester();
    }

    public class SemesterService : ISemesterService
    {
        public string GetCurrentSemester()
        {
            // L?y semester t? các l?p ?ang Open ho?c InProgress
            var currentSemester = FakeDatabase.CourseClasses
                .Where(c => c.Status == CourseClassStatus.Open || 
                           c.Status == CourseClassStatus.InProgress)
                .OrderByDescending(c => c.Semester)
                .FirstOrDefault()?.Semester;

            // Fallback n?u không tìm th?y
            if (string.IsNullOrEmpty(currentSemester))
            {
                currentSemester = FakeDatabase.CourseClasses
                    .OrderByDescending(c => c.Semester)
                    .FirstOrDefault()?.Semester ?? "HK1-2024";
            }

            return currentSemester;
        }

        public List<string> GetAllSemesters()
        {
            return FakeDatabase.CourseClasses
                .Select(c => c.Semester)
                .Distinct()
                .OrderByDescending(s => s)
                .ToList();
        }

        public bool IsEnrollmentOpen(string semester)
        {
            // Ki?m tra có l?p nào ?ang Open trong semester này không
            return FakeDatabase.CourseClasses
                .Any(c => c.Semester == semester && c.Status == CourseClassStatus.Open);
        }

        public string GetDefaultSemester()
        {
            // M?c ??nh tr? v? semester hi?n t?i
            return GetCurrentSemester();
        }
    }
}
