namespace IdentityServer.Exceptions;
public class ForbiddenException : Exception
{
    public ForbiddenException() : base("Bạn không có quyền thực hiện yêu cầu này!") { }

    public ForbiddenException(string message) : base(message)
    {

    }
}