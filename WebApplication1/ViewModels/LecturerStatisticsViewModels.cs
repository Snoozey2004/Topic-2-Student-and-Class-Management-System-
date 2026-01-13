using System;
using System.Collections.Generic;

namespace WebApplication1.ViewModels
{
    public class LecturerStatisticsViewModel
    {
        public int LecturerId { get; set; }
        public string LecturerName { get; set; } = string.Empty;

        public int TotalClasses { get; set; }
        public int TotalStudents { get; set; }

        public List<ClassGradeStatViewModel> GradeStats { get; set; } = new();
        public List<StudentNoScoreViewModel> StudentsWithoutScores { get; set; } = new();
        public List<ClassAttendanceStatViewModel> AttendanceStats { get; set; } = new();
    }

    public class ClassGradeStatViewModel
    {
        public int CourseClassId { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public double AverageScore { get; set; }

        public int ExcellentCount { get; set; }   // 9-10
        public int GoodCount { get; set; }        // 8-8.9
        public int FairCount { get; set; }        // 7-7.9
        public int AverageCount { get; set; }     // 5-6.9
        public int WeakCount { get; set; }        // <5
    }

    public class StudentNoScoreViewModel
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
    }

    public class ClassAttendanceStatViewModel
    {
        public int CourseClassId { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public double AverageAttendanceRate { get; set; }
        public int HighAbsenceCount { get; set; }

        public List<AttendanceAlertViewModel> HighAbsenceStudents { get; set; } = new();
        public List<SessionAttendanceDetailViewModel> SessionDetails { get; set; } = new();
    }

    public class AttendanceAlertViewModel
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public int PresentSessions { get; set; }
        public int AbsentSessions { get; set; }
        public double AttendanceRate { get; set; }
    }

    public class SessionAttendanceDetailViewModel
    {
        public DateTime SessionDate { get; set; }
        public string Session { get; set; } = string.Empty;
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public double AttendanceRate { get; set; }
    }
}
