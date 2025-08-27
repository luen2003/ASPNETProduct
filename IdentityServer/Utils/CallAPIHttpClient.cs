using System.Text;

namespace IdentityServer.Utils;

public class CallAPIHttpClient
{
    public static async Task<string> CallAPI(string url, string method, string? token, string data = "", int timeOutMilliseconds = 30000)
    {
        var httpContent = new StringContent(
            data,
            Encoding.UTF8,
            "application/json");
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", token));
        httpClient.Timeout = TimeSpan.FromMilliseconds(timeOutMilliseconds);

        try
        {

            var response = method.ToUpper() switch
            {
                "GET" => await httpClient.GetAsync(url),
                "POST" => await httpClient.PostAsync(url, httpContent),
                "PUT" => await httpClient.PutAsync(url, httpContent),
                "DELETE" => await httpClient.DeleteAsync(url),
                _ => throw new HttpRequestException("Method not found!")
            };

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}
