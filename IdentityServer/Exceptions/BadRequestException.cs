using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Exceptions;
public class BadRequestException : Exception
{
    public BadRequestException() : base("Không tìm thấy dữ liệu!") { }

    public BadRequestException(string message) : base(message)
    {

    }
}

public class BadRequestTest
{
    public static BadRequestObjectResult BadRequest([ActionResultObjectValue] string? error)
    {
        var response = new Response("400", error, null);
        return new BadRequestObjectResult(response);
    }

}