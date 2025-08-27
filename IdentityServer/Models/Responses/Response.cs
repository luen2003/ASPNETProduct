namespace IdentityServer.Models.Responses
{
    public class Response
    {
        public string Code { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? ResponseId { get; set; }
        public string? ResponseDate { get; set; }
        public object? Data { get; set; } = null!;

        public Response()
        {

        }

        public Response(string code, string message, string? responseId, string? responseDate, object? data)
        {
            Code = code;
            Message = message;
            ResponseId = responseId;
            ResponseDate = responseDate;
            Data = data;
        }

        public Response(string code, string message, object? data)
        {
            Code = code;
            Message = message;
            Data = data;
        }
    }
}
