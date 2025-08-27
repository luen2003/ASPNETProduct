namespace IdentityServer.Models;
public class Response
{
    public string? Code { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }

    public Response() { }

    public Response(string? code, string? message, object? data)
    {
        Code = code;
        Message = message;
        Data = data;
    }
}
