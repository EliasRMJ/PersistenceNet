using System.Net;

namespace PersistenceNet.Test.Middleware
{
    public record ExceptionResponse(HttpStatusCode StatusCode, string Description);
}