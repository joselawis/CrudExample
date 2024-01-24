using System.Net;
using Serilog;

namespace CrudExample.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IDiagnosticContext _diagnosticContext;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IDiagnosticContext diagnosticContext
    )
    {
        _next = next;
        _logger = logger;
        _diagnosticContext = diagnosticContext;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                _logger.LogError(
                    "{ExceptionType} {ExceptionMessage}",
                    ex.InnerException.GetType().ToString(),
                    ex.InnerException.Message
                );
            }
            else
            {
                _logger.LogError(
                    "{ExceptionType} {ExceptionMessage}",
                    ex.GetType().ToString(),
                    ex.Message
                );
            }

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await httpContext.Response.WriteAsync("Error occurred");
        }
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static void UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
