using Microsoft.AspNetCore.Mvc.Filters;

namespace CrudExample.Filters.ResultFilters;

public class TokenResultFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context) { }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        context.HttpContext.Response.Cookies.Append("Auth-Key", "A100");
    }
}
