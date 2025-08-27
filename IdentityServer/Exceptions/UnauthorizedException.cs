namespace IdentityServer.Exceptions;
public class UnauthorizedException : Exception
{
    public UnauthorizedException() : base("Bạn chưa đăng nhập") { }

    public UnauthorizedException(string message) : base(message)
    {

    }
}