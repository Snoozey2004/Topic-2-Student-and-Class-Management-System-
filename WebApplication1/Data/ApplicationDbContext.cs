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
    }
}
