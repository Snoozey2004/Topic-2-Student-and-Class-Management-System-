using System.Collections.Generic;

namespace WebApplication1.ViewModels
{
    public class GradeListViewModel
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string CourseClassCode { get; set; } = string.Empty;
        public double? TotalScore { get; set; }
        public string? LetterGrade { get; set; }
    }
}
