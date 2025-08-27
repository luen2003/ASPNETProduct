using Newtonsoft.Json;
using System.Text;

namespace IdentityServer.Helpers;
public class LoggingHelper
{
    private readonly IConfiguration _configuration;

    public LoggingHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> GetLogRequest(HttpContext context)
    {
        string body = await GetBodyRequest(context);
        string jwtEncodedString = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace(" ", "").Replace("Bearer", "") ?? "";
        var remoteIpAddress = context.Connection.RemoteIpAddress;
        string result =
        @$"RemoteIpAddress => {remoteIpAddress}
           Request => Method: {context.Request.Method}, Path: {context.Request.Path}, 
                      Token: {jwtEncodedString}, 
                      Body: {body}";
        return result;
    }

    private async Task<string> GetBodyRequest(HttpContext context)
    {
        string body = "";
        try
        {
            var endpointIgnoreLog = _configuration.GetSection("LoggerSetting: EndpointIgnoreLoggingRequest").Get<List<string>>() ?? new();
            endpointIgnoreLog = endpointIgnoreLog.Select(e => e = e.ToLower()).ToList();
            var endpoint = context.Request.Path.ToString().Split("/").Last().ToLower();
            if (!endpointIgnoreLog.Any() || !endpointIgnoreLog.Contains(endpoint))
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true);
                var requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0; // Fix err 400
                var requestBodyJson = JsonConvert.DeserializeObject(requestBody);
                body = JsonConvert.SerializeObject(requestBodyJson);
            }
            return body;
        }
        catch
        {
            return body;
        }
    }
}
