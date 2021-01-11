using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IndicoV2.Tests.TestUtils.System.Net
{
    using RequestPredicate = Predicate<HttpRequestMessage>;
    using ResponseFactory = Func<HttpRequestMessage, HttpResponseMessage>;
    using Handlers = List<(Predicate<HttpRequestMessage> RequestPredicate, Func<HttpRequestMessage, HttpResponseMessage> ResponseFactory)>;

    internal class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Handlers _handlers = new();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(Send(request));

        private HttpResponseMessage Send(HttpRequestMessage request)
        {
            var foundIndex = _handlers.FindIndex(h => h.RequestPredicate(request));

            if (foundIndex < 0)
            {
                throw new InvalidOperationException("Handler not set for this request.");
            }

            var handler = _handlers[foundIndex];
            
            return handler.ResponseFactory(request);
        }

        public void ForRequest(RequestPredicate requestPredicate, object response) =>
            ForRequest(requestPredicate, JsonConvert.SerializeObject(response));

        public void ForRequest(RequestPredicate requestPredicate, string responseContent) => ForRequest(requestPredicate,
            new HttpResponseMessage { Content = new StringContent(responseContent) });

        public void ForRequest(RequestPredicate requestPredicate, HttpResponseMessage response) =>
            ForRequest(requestPredicate, (req) => response);

        public void ForRequest(RequestPredicate requestPredicate, ResponseFactory responseFactory) =>
            _handlers.Add(new (requestPredicate, responseFactory));
    }
}
