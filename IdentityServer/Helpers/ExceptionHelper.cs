using IdentityServer.Exceptions;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace IdentityServer.Helpers;
public class ExceptionHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ShowEx(Exception ex) => string.Concat("Error: ", ex.Message, Environment.NewLine, "StackTrace: ", ex.StackTrace);

    public static Task HandleMiddlewareExceptionAsync(
            HttpContext context,
            Exception ex,
            ILogger logger,
            string requestLog,
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
    {
        string error = ShowEx(ex);

        // Default
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)httpStatusCode;
        var response = new Response("500", error, null);

        // Xử lý riêng các exception 
        switch (ex)
        {
            case ForbiddenException:
                {
                    response = new Response("403", ex.Message, null);
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    break;
                }
            case UnauthorizedException:
                {
                    response = new Response("401", ex.Message, null);
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                }
            case BadRequestException:
                {
                    response = new Response("400", ex.Message, null);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                }
        }

        var jsonResponse = JsonConvert.SerializeObject(response);
        logger.LogError(@"{requestLog}
                        {error}", requestLog, jsonResponse);
        return context.Response.WriteAsync(jsonResponse);
    }
}


