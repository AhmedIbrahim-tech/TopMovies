using Microsoft.AspNetCore.Diagnostics;
using TopMovies.ViewModels;
using System.Diagnostics;

namespace TopMovies.Controllers;

public class ErrorController : Controller
{
    [Route("/Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

        var viewModel = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            StatusCode = statusCode
        };

        return View("Error", viewModel);
    }

    [Route("/Error")]
    public IActionResult Error()
    {
        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        
        // Check if exception was stored by GlobalExceptionMiddleware
        var exception = HttpContext.Items["Exception"] as Exception;
        var exceptionMessage = HttpContext.Items["ExceptionMessage"] as string;

        var viewModel = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            ExceptionMessage = exceptionMessage ?? exceptionHandlerPathFeature?.Error?.Message,
            StatusCode = HttpContext.Response.StatusCode
        };

        return View("Error", viewModel);
    }
}

