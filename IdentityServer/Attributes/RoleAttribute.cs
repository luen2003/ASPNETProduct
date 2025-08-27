using IdentityServer.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdentityServer.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public abstract class RoleAttribute : Attribute, IAuthorizationFilter
{
    protected readonly RoleEnum[] _roles;
    protected string _userRole = string.Empty;
    protected bool _checkedRole = true;

    protected RoleAttribute(params RoleEnum[] roles)
    {
        _roles = roles;
    }

    public virtual void OnAuthorization(AuthorizationFilterContext context)
    {
        var jwtEncodedString =
            (context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()) ?? throw new UnauthorizedException();

        var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
        _userRole = token.Claims.FirstOrDefault(c => c.Type == "Role")?.Value ?? throw new UnauthorizedException();
        _checkedRole = _roles.Any(r => r.ToString() == _userRole);
    }
}

