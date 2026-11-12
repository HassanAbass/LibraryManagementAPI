using System.Diagnostics;

namespace LibraryManagementAPI.Middleware
{
    public class RequestTimingMiddleware(RequestDelegate _next)
    {
        public async Task InvokeAsync(HttpContext httpContext)
        {
            var stopwatch = Stopwatch.StartNew();
            httpContext.Response.OnStarting(() =>
            {
                stopwatch.Stop();
                httpContext.Response.Headers["X-Request-Duration"] = stopwatch.ElapsedMilliseconds.ToString();
                return Task.CompletedTask;
            });
             
            await _next(httpContext);
        }
    }
}
