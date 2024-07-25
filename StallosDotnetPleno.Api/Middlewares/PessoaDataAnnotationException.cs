using Microsoft.AspNetCore.Mvc;
using StallosDotnetPleno.Api.Security;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace StallosDotnetPleno.Api.Middlewares;

public class PessoaDataAnnotationException
{

    private readonly RequestDelegate _next;
    private readonly ILogger<PessoaDataAnnotationException> _logger;

    public PessoaDataAnnotationException(RequestDelegate next, ILogger<PessoaDataAnnotationException> logger)
    {
        _next = next;
        _logger = logger;
    }


    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (PessoaValidationException ex)
        {
            _logger.LogError($"Validation error: {ex}");
            await HandleValidationExceptionAsync(httpContext, ex);
        }
    }

    private static Task HandleValidationExceptionAsync(HttpContext context, PessoaValidationException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var problemDetails = new ValidationProblemDetails
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = "Uma ou mais validações resgatadas.",
            Instance = context.Request.Path,
            Detail = "Observe a estrutura de error e corrija."
        };

        foreach (var key in exception.Errors.Keys)
        {
            problemDetails.Errors.Add(key, exception.Errors[key].ToArray());
        }

        var result = JsonSerializer.Serialize(problemDetails);
        return context.Response.WriteAsync(result);
    }


}
