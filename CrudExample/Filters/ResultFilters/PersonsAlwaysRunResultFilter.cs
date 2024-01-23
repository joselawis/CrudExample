using Microsoft.AspNetCore.Mvc.Filters;

namespace CrudExample.Filters.ResultFilters;

public class PersonsAlwaysRunResultFilter : IAlwaysRunResultFilter
{
    private readonly ILogger<PersonsAlwaysRunResultFilter> _logger;

    public PersonsAlwaysRunResultFilter(ILogger<PersonsAlwaysRunResultFilter> logger)
    {
        _logger = logger;
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Filters.OfType<SkipFilter>().Any())
            return;
        _logger.LogInformation(
            "{FilterName}.{MethodName} - before",
            nameof(PersonsAlwaysRunResultFilter),
            nameof(OnResultExecuting)
        );
    }

    public void OnResultExecuted(ResultExecutedContext context) { }
}
