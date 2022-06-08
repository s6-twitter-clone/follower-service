using System.Net;

namespace follower_service.Exceptions;

public class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public AppException(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}
