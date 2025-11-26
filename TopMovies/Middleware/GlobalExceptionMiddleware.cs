using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using TopMovies.ViewModels;

namespace TopMovies.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred. RequestId: {RequestId}", context.TraceIdentifier);
            
            // If response has already started, we can't modify it
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response has already started, cannot handle exception");
                throw;
            }

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Check if the request is for an API endpoint (JSON expected)
        var isApiRequest = context.Request.Path.StartsWithSegments("/api") ||
                          context.Request.Headers.Accept.ToString().Contains("application/json");

        if (isApiRequest)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorViewModel
            {
                RequestId = context.TraceIdentifier,
                ExceptionMessage = _environment.IsDevelopment() 
                    ? exception.ToString() 
                    : "An error occurred while processing your request.",
                StatusCode = context.Response.StatusCode
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
        else
        {
            // For web requests, store exception info in HttpContext for ErrorController
            context.Items["Exception"] = exception;
            context.Items["ExceptionMessage"] = _environment.IsDevelopment() 
                ? exception.Message 
                : "An error occurred while processing your request.";
            
            // Redirect to Error controller
            context.Response.Redirect("/Error");
        }
    }
}

