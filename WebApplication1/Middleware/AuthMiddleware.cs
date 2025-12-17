using WebApplication1.Models;

namespace WebApplication1.Middleware
{
    /// <summary>
    /// Middleware x? lý Authentication và Authorization
    /// </summary>
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // Danh sách các path công khai (không c?n ??ng nh?p)
            var publicPaths = new[]
            {
                "/",
                "/home",
                "/account/login",
                "/account/register",
                "/account/forgotpassword",
                "/css",
                "/js",
                "/lib",
                "/favicon.ico"
            };

            // Ki?m tra n?u là public path
            if (publicPaths.Any(p => path.StartsWith(p)))
            {
                await _next(context);
                return;
            }

            // Ki?m tra ?ã ??ng nh?p ch?a
            var userIdClaim = context.User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                context.Response.Redirect("/Account/Login?returnUrl=" + Uri.EscapeDataString(path));
                return;
            }

            var userId = int.Parse(userIdClaim.Value);
            var roleClaim = context.User.FindFirst("Role");
            if (roleClaim == null)
            {
                context.Response.Redirect("/Account/Login");
                return;
            }

            var role = Enum.Parse<UserRole>(roleClaim.Value);

            // Ki?m tra quy?n truy c?p theo role
            if (path.StartsWith("/admin") && role != UserRole.Admin)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Access Denied");
                return;
            }

            if (path.StartsWith("/lecturer") && role != UserRole.Lecturer)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Access Denied");
                return;
            }

            if (path.StartsWith("/student") && role != UserRole.Student)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Access Denied");
                return;
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extension method ?? thêm AuthMiddleware vào pipeline
    /// </summary>
    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
