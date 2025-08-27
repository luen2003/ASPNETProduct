using IdentityServer.Helpers; 
using System.IdentityModel.Tokens.Jwt;

namespace IdentityServer.Authentication;
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtMiddleware> _logger;

    public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, LoggingHelper loggingHelper)
    {
        string? jwtEncodedString = null;
        string requestLog = await loggingHelper.GetLogRequest(context);
        try
        {
            jwtEncodedString = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace(" ", "").Replace("Bearer", "");
            if (!string.IsNullOrEmpty(jwtEncodedString))
            {
                var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
                var userName = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                var serviceCode = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId)?.Value;
                var userId = token.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                var role = token.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
                var posId = token.Claims.FirstOrDefault(c => c.Type == "PosId")?.Value;
                var plxId = token.Claims.FirstOrDefault(c => c.Type == "PlxId")?.Value;


                var applicationUser = context.RequestServices.GetRequiredService<ApplicationUser>();
                applicationUser.UserName = userName ?? string.Empty;
                applicationUser.ServiceCode = serviceCode ?? string.Empty;
                applicationUser.Token = jwtEncodedString;
                applicationUser.UserId = userId ?? string.Empty;
                applicationUser.Role = role ?? string.Empty;
                applicationUser.PlxId = plxId ?? string.Empty;
                if (!string.IsNullOrEmpty(posId))
                {
                    applicationUser.PosId = long.Parse(posId);
                }
            }
        }
        catch (Exception ex)
        {
            await ExceptionHelper.HandleMiddlewareExceptionAsync(context, ex, _logger, requestLog, HttpStatusCode.Unauthorized);
            return;
        }
        await Next(context, requestLog);
    }

    private async Task Next(HttpContext context, string requestLog)
    {
        Stream originalBody = context.Response.Body;
        try
        {
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            await _next(context);

            memStream.Position = 0;
            string responseBody = new StreamReader(memStream).ReadToEnd();
            var remoteIpAddress = context.Connection.RemoteIpAddress;
            _logger.LogInformation(
           @"{requestLog} 
           Response => StatusCode: {StatusCode}, Body: {responseBody}",
              requestLog, context.Response.StatusCode, responseBody);

            memStream.Position = 0;
            await memStream.CopyToAsync(originalBody);
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }
}
