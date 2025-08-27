using IdentityServer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using Response = IdentityServer.Models.Response;

namespace IdentityServer.Communication.Sync;
public class HttpClientService
{
    public static async Task<T?> Call<T>(
        string url, string token, HttpMethod method, string content, bool piacomResponse = true,
        int timeOutMilliseconds = 30000,
        string contentType = "application/json", Dictionary<string, string> contentForm = null!) where T : class
    {
        var responseBase = await CallBase(url, token, method, content, timeOutMilliseconds, contentType, contentForm);
        if (piacomResponse)
        {
            var responseData = JsonConvert.DeserializeObject<Response>(responseBase);
            if (responseData?.Data != null)
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(responseData.Data));
            return null;
        }
        return JsonConvert.DeserializeObject<T>(responseBase);
    }

    public static async Task<string> CallBase(string url, string token, HttpMethod method, string content,
        int timeOutMilliseconds = 30000, string contentType = "application/json", Dictionary<string, string> contentForm = null!)
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        using var httpClient = new HttpClient(clientHandler);

        // Thiết lập Header
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        httpClient.DefaultRequestHeaders.Add("Authentication", token);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
        httpClient.Timeout = TimeSpan.FromMilliseconds(timeOutMilliseconds);

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = method,
            Content = contentType == "application/json"
                            ? new StringContent(content, Encoding.UTF8, contentType)
                            : new FormUrlEncodedContent(contentForm)
        };

        HttpResponseMessage response = await httpClient.SendAsync(request);

        // Phát sinh Exception nếu mã trạng thái trả về là lỗi
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
