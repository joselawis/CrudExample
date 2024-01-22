using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CrudExample.Filters.AuthorizationFilters;

public class TokenAuthorizationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Cookies.ContainsKey("Auth-Key"))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
        }
        else if (context.HttpContext.Request.Cookies["Auth-Key"] != "A100")
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
        }
    }
}
