using System;
using System.Collections.Generic;

namespace WebApplication1.ViewModels
{
    public class StudentStatisticsViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;

        public int CurrentCourseCount { get; set; }
        public int TotalRegisteredCredits { get; set; }
        public List<WeeklyScheduleItemViewModel> ThisWeekSchedule { get; set; } = new();

        public double? GPA { get; set; }
        public List<CourseGradeStatViewModel> CourseGrades { get; set; } = new();
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }

        public AttendanceSummaryViewModel AttendanceSummary { get; set; } = new();
    }

    public class WeeklyScheduleItemViewModel
    {
        public DateTime Date { get; set; }
        public string Session { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
    }

    public class CourseGradeStatViewModel
    {
        public int CourseClassId { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public double? MidtermScore { get; set; }
        public double? FinalScore { get; set; }
        public double? TotalScore { get; set; }
        public string? LetterGrade { get; set; }
        public string Classification { get; set; } = string.Empty; // Xu?t s?c/Gi?i/Khá/Trung bình/Y?u
    }

    public class AttendanceSummaryViewModel
    {
        public double AverageRate { get; set; }
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
        public List<AttendanceCourseDetailViewModel> Courses { get; set; } = new();
        public List<AbsentDayViewModel> AbsentDays { get; set; } = new();
        public bool IsOverThreshold => AverageRate < 80; // c?nh báo <80% (v?ng >20%)
    }

    public class AttendanceCourseDetailViewModel
    {
        public int CourseClassId { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int PresentSessions { get; set; }
        public int TotalSessions { get; set; }
        public double AttendanceRate { get; set; }
        public int AbsentSessions => TotalSessions - PresentSessions;
    }

    public class AbsentDayViewModel
    {
        public DateTime Date { get; set; }
        public string Session { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
}
