using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    /// <summary>
    /// Service qu?n lý Subject
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
        public List<SubjectListViewModel> GetAll()
        {
            return FakeDatabase.Subjects.Select(s => new SubjectListViewModel
            {
                Id = s.Id,
                SubjectCode = s.SubjectCode,
                SubjectName = s.SubjectName,
                Credits = s.Credits,
                Department = s.Department,
                PrerequisiteCount = s.PrerequisiteSubjectIds.Count
            }).OrderBy(s => s.SubjectCode).ToList();
        }

        public SubjectDetailViewModel? GetDetailById(int id)
        {
            var subject = GetById(id);
            if (subject == null) return null;

            var prerequisites = subject.PrerequisiteSubjectIds
                .Select(pid => FakeDatabase.Subjects.FirstOrDefault(s => s.Id == pid))
                .Where(s => s != null)
                .Select(s => $"{s!.SubjectCode} - {s.SubjectName}")
                .ToList();

            var courseClasses = FakeDatabase.CourseClasses
                .Where(c => c.SubjectId == id)
                .Select(c =>
                {
                    var lecturer = FakeDatabase.Lecturers.FirstOrDefault(l => l.Id == c.LecturerId);
                    return new CourseClassInfoViewModel
                    {
                        Id = c.Id,
                        ClassCode = c.ClassCode,
                        SubjectName = subject.SubjectName,
                        Semester = c.Semester,
                        CurrentStudents = c.CurrentStudents,
                        MaxStudents = c.MaxStudents,
                        Status = c.Status.ToString()
                    };
                }).ToList();

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
            return FakeDatabase.Subjects.FirstOrDefault(s => s.Id == id);
        }

        public bool Create(SubjectFormViewModel model)
        {
            // Ki?m tra mã môn h?c ?ã t?n t?i
            if (FakeDatabase.Subjects.Any(s => s.SubjectCode.Equals(model.SubjectCode, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            var subject = new Subject
            {
                Id = FakeDatabase.GetNextSubjectId(),
                SubjectCode = model.SubjectCode,
                SubjectName = model.SubjectName,
                Credits = model.Credits,
                Department = model.Department,
                Description = model.Description,
                PrerequisiteSubjectIds = model.PrerequisiteSubjectIds ?? new List<int>(),
                CreatedDate = DateTime.Now
            };

            FakeDatabase.Subjects.Add(subject);
            return true;
        }

        public bool Update(SubjectFormViewModel model)
        {
            var subject = GetById(model.Id ?? 0);
            if (subject == null) return false;

            // Ki?m tra mã môn h?c trùng
            if (FakeDatabase.Subjects.Any(s => s.Id != subject.Id &&
                s.SubjectCode.Equals(model.SubjectCode, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            subject.SubjectCode = model.SubjectCode;
            subject.SubjectName = model.SubjectName;
            subject.Credits = model.Credits;
            subject.Department = model.Department;
            subject.Description = model.Description;
            subject.PrerequisiteSubjectIds = model.PrerequisiteSubjectIds ?? new List<int>();

            return true;
        }

        public bool Delete(int id)
        {
            var subject = GetById(id);
            if (subject == null) return false;

            // Ki?m tra xem môn h?c có ?ang ???c s? d?ng không
            if (FakeDatabase.CourseClasses.Any(c => c.SubjectId == id))
            {
                return false;
            }

            FakeDatabase.Subjects.Remove(subject);
            return true;
        }

        public List<Subject> GetPrerequisites(int subjectId)
        {
            var subject = GetById(subjectId);
            if (subject == null) return new List<Subject>();

            return subject.PrerequisiteSubjectIds
                .Select(pid => GetById(pid))
                .Where(s => s != null)
                .Cast<Subject>()
                .ToList();
        }
    }
}
