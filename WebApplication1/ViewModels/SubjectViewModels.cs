using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    /// <summary>
    /// ViewModel for creating/editing Subject
    /// </summary>
    public class SubjectFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Subject code is required")]
        public string SubjectCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject name is required")]
        public string SubjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Credits is required")]
        [Range(1, 10, ErrorMessage = "Credits must be between 1-10")]
        public int Credits { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public string Department { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<int> PrerequisiteSubjectIds { get; set; } = new List<int>();
    }

    /// <summary>
    /// ViewModel for Subject list
    /// </summary>
    public class SubjectListViewModel
    {
        public int Id { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string Department { get; set; } = string.Empty;
        public int PrerequisiteCount { get; set; }
    }

    /// <summary>
    /// ViewModel for Subject details
    /// </summary>
    public class SubjectDetailViewModel
    {
        public int Id { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string Department { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> PrerequisiteSubjects { get; set; } = new List<string>();
        public List<CourseClassInfoViewModel> CourseClasses { get; set; } = new List<CourseClassInfoViewModel>();
    }
}
