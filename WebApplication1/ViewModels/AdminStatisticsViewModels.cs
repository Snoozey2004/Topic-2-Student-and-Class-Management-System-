using System;
using System.Collections.Generic;

namespace WebApplication1.ViewModels
{
    public class AdminStatisticsViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalLecturers { get; set; }
        public int TotalClasses { get; set; }

        public double AverageScore { get; set; }
        public double PassRate { get; set; }
        public List<ScoreDistributionItem> ScoreDistribution { get; set; } = new();
        public List<TopStudentItem> TopStudents { get; set; } = new();
        public int WarningLowScoreCount { get; set; }
        public List<SemesterTrendItem> ScoreTrendBySemester { get; set; } = new();

        public double AverageAttendanceRate { get; set; }
        public int HighAbsenceStudentCount { get; set; }
        public List<AttendanceDistributionItem> AttendanceByClass { get; set; } = new();
        public List<SemesterTrendItem> AttendanceTrendBySemester { get; set; } = new();
    }

    public class ScoreDistributionItem
    {
        public string Label { get; set; } = string.Empty; // 9-10, 8-8.9, ...
        public int Count { get; set; }
    }

    public class TopStudentItem
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public double GPA { get; set; }
    }

    public class AttendanceDistributionItem
    {
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public double AttendanceRate { get; set; }
        public int StudentCount { get; set; }
    }

    public class SemesterTrendItem
    {
        public string Semester { get; set; } = string.Empty;
        public double Value { get; set; }
    }
}
