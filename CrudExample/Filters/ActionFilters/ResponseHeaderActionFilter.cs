using Microsoft.AspNetCore.Mvc.Filters;

namespace CrudExample.Filters.ActionFilters;

public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter
{
    public string? Key { get; set; }
    public string? Value { get; set; }
    public int Order { get; set; }

    private readonly ILogger<ResponseHeaderActionFilter> _logger;

    public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        _logger.LogInformation(
            "{FilterName}.{MethodName} method called - before",
            nameof(ResponseHeaderActionFilter),
            nameof(OnActionExecutionAsync)
        );

        await next(); // Calls the subsequent filter or action method

        _logger.LogInformation(
            "{FilterName}.{MethodName} method called - after",
            nameof(ResponseHeaderActionFilter),
            nameof(OnActionExecutionAsync)
        );
        context.HttpContext.Response.Headers[Key] = Value;
    }
}
