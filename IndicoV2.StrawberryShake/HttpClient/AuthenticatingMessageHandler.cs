using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IndicoV2.StrawberryShake.HttpClient
{
    public class AuthenticatingMessageHandler : HttpClientHandler
    {
        private readonly Uri _refreshUri;
        private readonly string _refreshToken;
        private string _token;

        public AuthenticatingMessageHandler(Uri baseUri, string refreshToken)
        {
            _refreshUri = new Uri(baseUri, "/auth/users/refresh_token");
            _refreshToken = refreshToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshTokenAsync(cancellationToken);
                response = await base.SendAsync(request, cancellationToken);
            }

            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedAccessException(response.ReasonPhrase),
                _ => response,
            };
        }

        private async Task RefreshTokenAsync(CancellationToken cancellationToken)
        {
            var refreshTokenRequest = new HttpRequestMessage(HttpMethod.Post, _refreshUri);
            refreshTokenRequest.Headers.Authorization = GetAuthHeader(_refreshToken);
            var responseMessage = await base.SendAsync(refreshTokenRequest, cancellationToken);

            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new AuthenticationException(responseMessage.ReasonPhrase);
            }

            var responseStream = await responseMessage.Content.ReadAsStreamAsync();
            var response =
                await JsonSerializer.DeserializeAsync<RefreshTokenResponse>(responseStream, null, cancellationToken);

            _token = response.AuthToken ?? throw new AuthenticationException($"Cannot find {nameof(response.AuthToken)}.");
        }

        private AuthenticationHeaderValue GetAuthHeader(string token) => new AuthenticationHeaderValue("Bearer", token);
    }
}
