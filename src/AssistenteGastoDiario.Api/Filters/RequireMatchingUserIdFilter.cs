using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AssistenteGastoDiario.Api.Filters;

public sealed class RequireMatchingUserIdFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var routeValue = context.RouteData.Values["userId"]?.ToString();
        var claimValue = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(routeValue, out var routeUserId)
            || !Guid.TryParse(claimValue, out var authenticatedUserId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (routeUserId != authenticatedUserId)
        {
            context.Result = new ForbidResult();
        }
    }
}
