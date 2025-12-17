using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    /// <summary>
    /// ViewModel cho t?o/ch?nh s?a Schedule
    /// </summary>
    public class ScheduleFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "L?p h?c ph?n là b?t bu?c")]
        public int CourseClassId { get; set; }

        [Required(ErrorMessage = "Th? là b?t bu?c")]
        public DayOfWeek DayOfWeek { get; set; }

        [Required(ErrorMessage = "Bu?i h?c là b?t bu?c")]
        public string Session { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ca h?c là b?t bu?c")]
        public string Period { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gi? b?t ??u là b?t bu?c")]
        public string StartTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gi? k?t thúc là b?t bu?c")]
        public string EndTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phòng h?c là b?t bu?c")]
        public string Room { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày b?t ??u áp d?ng là b?t bu?c")]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// ViewModel cho danh sách Schedule
    /// </summary>
    public class ScheduleListViewModel
    {
        public int Id { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string DayOfWeek { get; set; } = string.Empty;
        public string Session { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public string TimeRange { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public DateTime EffectiveDate { get; set; }
    }

    /// <summary>
    /// ViewModel cho th?i khóa bi?u
    /// </summary>
    public class TimetableViewModel
    {
        public string Semester { get; set; } = string.Empty;
        public Dictionary<DayOfWeek, List<TimetableSlot>> Schedule { get; set; } = new Dictionary<DayOfWeek, List<TimetableSlot>>();
    }

    /// <summary>
    /// ViewModel cho m?t slot trong th?i khóa bi?u
    /// </summary>
    public class TimetableSlot
    {
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string LecturerName { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public string TimeRange { get; set; } = string.Empty;
    }
}
