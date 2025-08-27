namespace ProductView.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAndGetTokenAsync(string username, string password, CancellationToken ct = default);
    }
}
