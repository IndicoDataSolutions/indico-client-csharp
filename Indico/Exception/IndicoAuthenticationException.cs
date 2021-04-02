using System.Net;
using System.Net.Http;

namespace Indico.Exception
{
    public class IndicoAuthenticationException : HttpRequestException
    {
        public HttpStatusCode StatusCode { get; }

        public IndicoAuthenticationException(string message, HttpStatusCode statusCode)
            : base(message) =>
            StatusCode = statusCode;
    }
}
