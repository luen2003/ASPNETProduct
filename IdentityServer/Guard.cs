global using IdentityServer.Enums;
using IdentityServer.Authentication;
using IdentityServer.Exceptions;

namespace IdentityServer;
public class Guard
{
    public static void ThrowIfArgumentNull(object argument, string argumentName)
    {
        if (argument == null)
        {
            throw new ArgumentNullException(argumentName, "The given argument is null");
        }
    }

    public static void ThrowIfForbidden(ApplicationUser user, RoleEnum roleAceepted)
    {
        if (user.Role != roleAceepted.ToString())
        {
            throw new ForbiddenException("Bạn không có quyền thực hiện yêu cầu này!");
        }
    }
}