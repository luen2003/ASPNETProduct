namespace ProductView.Models
{
public class ApiResponse<T>
{
    public string Code { get; set; } = ""; // cho non-nullable
    public string Message { get; set; } = "";
    public T Data { get; set; } = default!;
    public object? Error { get; set; }
}

}
