using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Lecturer> Lecturers => Set<Lecturer>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<CourseClass> CourseClasses => Set<CourseClass>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<Grade> Grades => Set<Grade>();
        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<AdministrativeClass> AdministrativeClasses => Set<AdministrativeClass>();
        public DbSet<Schedule> Schedules => Set<Schedule>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== UNIQUE (khuyên dùng) ==========
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.StudentCode)
                .IsUnique();

            modelBuilder.Entity<Lecturer>()
                .HasIndex(l => l.LecturerCode)
                .IsUnique();

            modelBuilder.Entity<Subject>()
                .HasIndex(s => s.SubjectCode)
                .IsUnique();

            modelBuilder.Entity<CourseClass>()
                .HasIndex(c => c.ClassCode)
                .IsUnique();

            // ========== 1-1: User - Student ==========
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== 1-1: User - Lecturer ==========
            modelBuilder.Entity<Lecturer>()
                .HasOne(l => l.User)
                .WithOne(u => u.Lecturer)
                .HasForeignKey<Lecturer>(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== AdministrativeClass - AdvisorLecturer (optional) ==========
            modelBuilder.Entity<AdministrativeClass>()
                .HasOne(a => a.AdvisorLecturer)
                .WithMany(l => l.AdvisoryClasses)
                .HasForeignKey(a => a.AdvisorLecturerId)
                .OnDelete(DeleteBehavior.SetNull);

            // ========== Student - AdministrativeClass (optional) ==========
            modelBuilder.Entity<Student>()
                .HasOne(s => s.AdministrativeClass)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.AdministrativeClassId)
                .OnDelete(DeleteBehavior.SetNull);

            // ========== CourseClass - Subject ==========
            modelBuilder.Entity<CourseClass>()
                .HasOne(c => c.Subject)
                .WithMany(s => s.CourseClasses)
                .HasForeignKey(c => c.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== CourseClass - Lecturer ==========
            modelBuilder.Entity<CourseClass>()
                .HasOne(c => c.Lecturer)
                .WithMany(l => l.CourseClasses)
                .HasForeignKey(c => c.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== Schedule - CourseClass ==========
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.CourseClass)
                .WithMany(c => c.Schedules)
                .HasForeignKey(s => s.CourseClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== Enrollment - Student ==========
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== Enrollment - CourseClass ==========
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.CourseClass)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== Enrollment - ApprovedBy(User) optional ==========
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.ApprovedByUser)
                .WithMany(u => u.ApprovedEnrollments)
                .HasForeignKey(e => e.ApprovedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // ========== Attendance ==========
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Enrollment)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.CourseClass)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.CourseClassId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Lecturer)
                .WithMany(l => l.CreatedAttendances)
                .HasForeignKey(a => a.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== Grade ==========
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Enrollment)
                .WithMany(e => e.Grades)
                .HasForeignKey(g => g.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.CourseClass)
                .WithMany(c => c.Grades)
                .HasForeignKey(g => g.CourseClassId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.UpdatedByLecturer)
                .WithMany(l => l.UpdatedGrades)
                .HasForeignKey(g => g.UpdatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // ========== Notification - User ==========
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // (Optional) tránh trùng attendance theo buổi học
            modelBuilder.Entity<Attendance>()
                .HasIndex(a => new { a.EnrollmentId, a.AttendanceDate, a.Session })
                .IsUnique();
        }
    }
}
