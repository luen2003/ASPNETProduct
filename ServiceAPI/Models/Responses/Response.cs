namespace TransactionService.Models.Responses;

public class Response
{
    public Response()
    {

    }
    public Response(string code, string message, object? data)
    {
        Code = code;
        Message = message;
        Data = data;
    }
    public string Code { get; set; } = null!;
    public string Message { get; set; } = null!;
    public object? Data { get; set; }
}
