using Microsoft.AspNetCore.Mvc.Filters;

namespace CrudExample.Filters.ActionFilters;

public class ResponseHeaderActionFilter : IActionFilter
{
    private readonly string _key;
    private readonly ILogger<ResponseHeaderActionFilter> _logger;
    private readonly string _value;

    public ResponseHeaderActionFilter(
        ILogger<ResponseHeaderActionFilter> logger,
        string key,
        string value
    )
    {
        _logger = logger;
        _key = key;
        _value = value;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation(
            "{FilterName}.{MethodName} method called",
            nameof(ResponseHeaderActionFilter),
            nameof(OnActionExecuting)
        );
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation(
            "{FilterName}.{MethodName} method called",
            nameof(ResponseHeaderActionFilter),
            nameof(OnActionExecuted)
        );

        context.HttpContext.Response.Headers[_key] = _value;
    }
}
