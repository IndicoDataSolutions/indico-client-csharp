using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace IndicoV2.StrawberryShake.HttpClient
{
    public class HttpSsClient : ErrorHandlingWrapper
    {
        private readonly ServiceProvider _services;

        public HttpSsClient(ServiceProvider services) => _services = services;

        public AuthenticatingMessageHandler GetHandler()
        {
            return _services.GetService<AuthenticatingMessageHandler>();
        }

    }
}
