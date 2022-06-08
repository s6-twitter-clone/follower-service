using System.Net;

namespace follower_service.Exceptions;

public class BadRequestException : AppException
{
    public BadRequestException(string message) : base(HttpStatusCode.BadRequest, message) { }
}
