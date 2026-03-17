using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DotNetDemoApi.Middleware
{
    /// <summary>
    /// Adds common security headers to all responses.
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["X-Frame-Options"] = "DENY";
                context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
