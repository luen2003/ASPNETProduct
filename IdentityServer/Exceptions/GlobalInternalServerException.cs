using IdentityServer.Helpers;

namespace IdentityServer.Middlewares;
public class GlobalInternalServerExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalInternalServerExceptionMiddleware> _logger;

    public GlobalInternalServerExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalInternalServerExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, LoggingHelper loggingHelper)
    {
        string logRequest = "";
        try
        {
            logRequest = await loggingHelper.GetLogRequest(context);
            await _next(context);
        }
        catch (Exception ex)
        {
            await ExceptionHelper.HandleMiddlewareExceptionAsync(context, ex, _logger, logRequest);
        }
    }
}
