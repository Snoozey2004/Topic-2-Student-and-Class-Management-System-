namespace WebApplication1.Models
{
    /// <summary>
    /// Model ??i di?n cho môn h?c
    /// </summary>
    public class Subject
    {
        public int Id { get; set; }
        public string SubjectCode { get; set; } = string.Empty; // VD: IT001
        public string SubjectName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string Department { get; set; } = string.Empty; // Khoa ph? trách
        public string Description { get; set; } = string.Empty;
        public List<int> PrerequisiteSubjectIds { get; set; } = new List<int>(); // Môn tiên quy?t
        public DateTime CreatedDate { get; set; }
    }
}
