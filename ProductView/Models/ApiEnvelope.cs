namespace ProductView.Models;

public class ApiEnvelope<T>
{
    public string? code { get; set; }
    public string? message { get; set; }
    public T? data { get; set; }
}
