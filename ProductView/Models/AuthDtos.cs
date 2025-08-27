namespace WebClient.Models;


public class AuthRequest
{
    public string username { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
}


public class AuthData
{
    public string? requestDate { get; set; }
    public string? serviceName { get; set; }
    public int status { get; set; }
    public string? token { get; set; }
}


public class ApiEnvelope<T>
{
    public string? code { get; set; }
    public string? message { get; set; }
    public T? data { get; set; }
    public object? error { get; set; }
}