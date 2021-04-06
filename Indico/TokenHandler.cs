using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Indico.Exception;
using Newtonsoft.Json;

namespace Indico
{
    internal class TokenHandler : DelegatingHandler
    {
        private readonly string _apiToken;

        public TokenHandler(string apiToken, HttpMessageHandler innerHandler) : base(innerHandler) => _apiToken = apiToken;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var httpContent = request.Content;
                var requestUri = request.RequestUri;
                string token = await GetToken(request, cancellationToken);
                request.Content = httpContent;
                request.RequestUri = requestUri;
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await base.SendAsync(request, cancellationToken);
                return response;
            }
            else
            {
                throw new HttpRequestException($"Error occured while calling authentication server: {response.StatusCode}.");
            }
        }

        private async Task<string> GetToken(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string endpoint = request.RequestUri.GetLeftPart(System.UriPartial.Authority);
            request.RequestUri = new System.Uri($"{endpoint}/auth/users/refresh_token");
            request.Content = null;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);

            var httpResponseMessage = await base.SendAsync(request, cancellationToken);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string json = await httpResponseMessage.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<Response>(json);
                return response.Token;
            }
            else
            {
                throw new IndicoAuthenticationException($"Error occured while calling authentication server: {httpResponseMessage.StatusCode}", httpResponseMessage.StatusCode);
            }
        }

        private class Response
        {
            [JsonProperty("auth_token")]
            public string Token { get; set; }
        }
    }
}