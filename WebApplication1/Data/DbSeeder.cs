using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            await db.Database.MigrateAsync();

            // đã có data thì thôi
            if (await db.Users.AnyAsync()) return;

            var now = DateTime.Now;
            var today = DateTime.Today;

            // =========================
            // 1) USERS
            // =========================
            var admin = new User
            {
                Email = "admin@university.edu.vn",
                Password = "admin123",
                FullName = "System Administrator",
                Role = UserRole.Admin,
                Status = UserStatus.Active,
                CreatedDate = now.AddYears(-1)
            };

        }
    }
}
