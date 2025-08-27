using IdentityServer.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdentityServer.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RoleRejectedAttribute : RoleAttribute
{
    public RoleRejectedAttribute(params RoleEnum[] roles) : base(roles)
    {

    }

    public override void OnAuthorization(AuthorizationFilterContext context)
    {
        base.OnAuthorization(context);
        if (_checkedRole) throw new ForbiddenException();
    }
}
