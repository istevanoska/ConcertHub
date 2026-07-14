using Microsoft.EntityFrameworkCore;
using Repository;

namespace Web.Middleware;

public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext db)
    {
        if (!context.Request.Path.StartsWithSegments("/api/external"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var key) || string.IsNullOrWhiteSpace(key))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("API key is required");
            return;
        }

        var apiKey = key.ToString();
        var client = await db.ApiClients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ApiKey == apiKey && c.IsActive);

        if (client == null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid API key");
            return;
        }

        context.Items["ApiClientId"] = client.Id;
        await _next(context);
    }
}
