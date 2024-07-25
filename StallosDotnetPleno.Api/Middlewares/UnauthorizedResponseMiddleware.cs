using System.Net;

namespace StallosDotnetPleno.Api.Middlewares;

public class UnauthorizedResponseMiddleware
{
    private readonly RequestDelegate _next;

    public UnauthorizedResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"message\":\"Unauthorized access.\"}");
        }
    }
}
