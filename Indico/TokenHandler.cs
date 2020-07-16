using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Indico
{
    class TokenHandler : DelegatingHandler
    {
        string _apiToken;

        public TokenHandler(string apiToken, HttpMessageHandler innerHandler) : base(innerHandler)
        {
            this._apiToken = apiToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContent httpContent = request.Content;
                System.Uri requestUri = request.RequestUri;
                string token = await GetToken(request, cancellationToken);
                request.Content = httpContent;
                request.RequestUri = requestUri;
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await base.SendAsync(request, cancellationToken);
                return response;
            }
            else
            {
                throw new HttpRequestException($"[error] : {response.StatusCode}");
            }
        }

        async Task<string> GetToken(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string endpoint = request.RequestUri.GetLeftPart(System.UriPartial.Authority);
            request.RequestUri = new System.Uri($"{endpoint}/auth/users/refresh_token");
            request.Content = null;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this._apiToken);

            HttpResponseMessage httpResponseMessage = await base.SendAsync(request, cancellationToken);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string json = await httpResponseMessage.Content.ReadAsStringAsync();
                Response response = JsonConvert.DeserializeObject<Response>(json);
                return response.Token;
            }
            else
            {
                throw new HttpRequestException($"[error] : {httpResponseMessage.StatusCode}");
            }
        }

        class Response
        {
            [JsonProperty("auth_token")]
            public string Token { get; set; }
        }
    }
}