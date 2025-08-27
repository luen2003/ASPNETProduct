namespace TransactionService.Models.Responses;

public class TransactionResponse
{
    public TransactionResponse()
    {

    }
    public TransactionResponse(string code, string message, object? data)
    {
        Code = code;
        Message = message;
        Data = data;
    }
    public string Code { get; set; } = null!;
    public string Message { get; set; } = null!;
    public object? Data { get; set; }
    public object? QHM { get; set; }
}
