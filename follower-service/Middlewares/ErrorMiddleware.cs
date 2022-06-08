using follower_service.Exceptions;
using System.Net;
using System.Text.Json;

namespace follower_service.Middlewares;

public class ErrorMiddleware
{
    private readonly RequestDelegate next;

    public ErrorMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            if (error is AppException applicationError)
            {
                response.StatusCode = (int)applicationError.StatusCode;
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            var result = JsonSerializer.Serialize(new { message = error.Message });
            await response.WriteAsync(result);
        }
    }
}
