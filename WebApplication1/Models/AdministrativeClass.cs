namespace WebApplication1.Models
{
    /// <summary>
    /// Model ??i di?n cho l?p hành chính
    /// </summary>
    public class AdministrativeClass
    {
        public int Id { get; set; }
        public string ClassName { get; set; } = string.Empty; // VD: CNTT-K17A
        public string Major { get; set; } = string.Empty;
        public int AdmissionYear { get; set; }
        public int? AdvisorLecturerId { get; set; } // Gi?ng viên c? v?n
        public DateTime CreatedDate { get; set; }
        // Navigation
        public Lecturer? AdvisorLecturer { get; set; }
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
