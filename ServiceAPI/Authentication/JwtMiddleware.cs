using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ServiceAPI.Authentication
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await LoggingRequest(context);
                var jwtEncodedString = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (jwtEncodedString != null)
                {
                    var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);

                    var userName = token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                    var serviceCode = token.Claims.First(c => c.Type == JwtRegisteredClaimNames.NameId).Value;
                    //var email = token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value;
                    _logger.LogInformation("userId: =======> {}", serviceCode);
                    if (serviceCode != null)
                    {
                        var applicationUser = context.RequestServices.GetRequiredService<ApplicationUser>();
                        applicationUser.UserName = userName;
                        applicationUser.ServiceCode = serviceCode;
                        applicationUser.Token = jwtEncodedString;
                    }
                    else
                    {
                        throw new Exception("Token không hợp lệ!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            await _next(context);
        }

        private async Task LoggingRequest(HttpContext context)
        {
            var endpointIgnoreLog = _configuration.GetSection("LoggerSetting:EndpointIgnoreLoggingRequest").Get<List<string>>() ?? new();
            endpointIgnoreLog = endpointIgnoreLog.Select(e => e = e.ToLower()).ToList();
            var endpoint = context.Request.Path.ToString().Split("/").Last().ToLower();
            if (!endpointIgnoreLog.Any() || !endpointIgnoreLog.Contains(endpoint))
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true);
                var requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0; // Fix err 400
                var requestBodyJson = JsonConvert.DeserializeObject(requestBody);
                string body = JsonConvert.SerializeObject(requestBodyJson);
                _logger.LogInformation("Request => Method: {Method}, Path: {Path}, Body: {body}", context.Request.Method, context.Request.Path, body);
            }
        }
    }
}
