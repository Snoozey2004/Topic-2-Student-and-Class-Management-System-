using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service quản lý Subject (SQL Server + EF Core)
    /// </summary>
    public interface ISubjectService
    {
        List<SubjectListViewModel> GetAll();
        SubjectDetailViewModel? GetDetailById(int id);
        Subject? GetById(int id);
        bool Create(SubjectFormViewModel model);
        bool Update(SubjectFormViewModel model);
        bool Delete(int id);
        List<Subject> GetPrerequisites(int subjectId);
    }

    public class SubjectService : ISubjectService
    {
        private readonly ApplicationDbContext _db;

        public SubjectService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<SubjectListViewModel> GetAll()
        {
            // Nếu PrerequisiteSubjectIds là List<int> map được thì dùng Count()
            return _db.Subjects
                .AsNoTracking()
                .Select(s => new SubjectListViewModel
                {
                    Id = s.Id,
                    SubjectCode = s.SubjectCode,
                    SubjectName = s.SubjectName,
                    Credits = s.Credits,
                    Department = s.Department,
                    PrerequisiteCount = s.PrerequisiteSubjectIds != null ? s.PrerequisiteSubjectIds.Count : 0
                })
                .OrderBy(s => s.SubjectCode)
                .ToList();
        }

        public SubjectDetailViewModel? GetDetailById(int id)
        {
            var subject = _db.Subjects
                .AsNoTracking()
                .FirstOrDefault(s => s.Id == id);

            if (subject == null) return null;

            // Load prerequisite subjects
            var prerequisiteIds = subject.PrerequisiteSubjectIds ?? new List<int>();

            var prerequisites = _db.Subjects
                .AsNoTracking()
                .Where(s => prerequisiteIds.Contains(s.Id))
                .Select(s => $"{s.SubjectCode} - {s.SubjectName}")
                .ToList();

            // CourseClasses của subject
            var courseClasses = _db.CourseClasses
                .AsNoTracking()
                .Where(c => c.SubjectId == id)
                .Select(c => new CourseClassInfoViewModel
                {
                    Id = c.Id,
                    ClassCode = c.ClassCode,
                    SubjectName = subject.SubjectName,
                    Semester = c.Semester,
                    CurrentStudents = c.CurrentStudents,
                    MaxStudents = c.MaxStudents,
                    Status = c.Status.ToString()
                })
                .ToList();

            return new SubjectDetailViewModel
            {
                Id = subject.Id,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Credits = subject.Credits,
                Department = subject.Department,
                Description = subject.Description,
                PrerequisiteSubjects = prerequisites,
                CourseClasses = courseClasses
            };
        }

        public Subject? GetById(int id)
        {
            return _db.Subjects.FirstOrDefault(s => s.Id == id);
        }

        public bool Create(SubjectFormViewModel model)
        {
            var codeLower = model.SubjectCode.Trim().ToLower();
            if (_db.Subjects.Any(s => s.SubjectCode.ToLower() == codeLower))
                return false;

            var subject = new Subject
            {
                // KHÔNG set Id nếu Identity
                SubjectCode = model.SubjectCode.Trim(),
                SubjectName = model.SubjectName,
                Credits = model.Credits,
                Department = model.Department,
                Description = model.Description,
                PrerequisiteSubjectIds = model.PrerequisiteSubjectIds ?? new List<int>(),
                CreatedDate = DateTime.Now
            };

            _db.Subjects.Add(subject);
            _db.SaveChanges();
            return true;
        }

        public bool Update(SubjectFormViewModel model)
        {
            var id = model.Id ?? 0;
            var subject = _db.Subjects.FirstOrDefault(s => s.Id == id);
            if (subject == null) return false;

            var codeLower = model.SubjectCode.Trim().ToLower();
            if (_db.Subjects.Any(s => s.Id != subject.Id && s.SubjectCode.ToLower() == codeLower))
                return false;

            subject.SubjectCode = model.SubjectCode.Trim();
            subject.SubjectName = model.SubjectName;
            subject.Credits = model.Credits;
            subject.Department = model.Department;
            subject.Description = model.Description;
            subject.PrerequisiteSubjectIds = model.PrerequisiteSubjectIds ?? new List<int>();

            _db.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var subject = _db.Subjects.FirstOrDefault(s => s.Id == id);
            if (subject == null) return false;

            // Không cho xoá nếu đang được dùng bởi CourseClass
            if (_db.CourseClasses.Any(c => c.SubjectId == id))
                return false;

            _db.Subjects.Remove(subject);
            _db.SaveChanges();
            return true;
        }

        public List<Subject> GetPrerequisites(int subjectId)
        {
            var subject = _db.Subjects
                .AsNoTracking()
                .FirstOrDefault(s => s.Id == subjectId);

            if (subject == null) return new List<Subject>();

            var prerequisiteIds = subject.PrerequisiteSubjectIds ?? new List<int>();
            if (prerequisiteIds.Count == 0) return new List<Subject>();

            return _db.Subjects
                .AsNoTracking()
                .Where(s => prerequisiteIds.Contains(s.Id))
                .ToList();
        }
    }
}
