using System.Net;

namespace follower_service.Exceptions;

public class InternalServerErrorException : AppException
{
    public InternalServerErrorException(string message) : base(HttpStatusCode.InternalServerError, message) { }
}
