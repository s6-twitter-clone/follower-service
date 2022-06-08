using System.Net;

namespace follower_service.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }
}
