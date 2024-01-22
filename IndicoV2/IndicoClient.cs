using System;
using System.Net;
using System.Net.Http;
using IndicoV2.Exception;
using IndicoV2.StrawberryShake;
using IndicoV2.StrawberryShake.HttpClient;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using IndicoV2.GraphQLRequest;

namespace IndicoV2
{
    /// <summary>
    /// Client for the indico.io platform.
    /// </summary>
    public class IndicoClient : IIndicoClient
    {
        /// <summary>https://app.indico.io</summary>
        private const string _defaultUrl = "https://app.indico.io";

        internal readonly string _apiToken;

        /// <summary>
        /// Gets the underlying http client.
        /// </summary>
        public HttpClient HttpClient { get; }

        /// <summary>
        /// Gets the underlying GraphQL client.
        /// </summary>
        public GraphQLHttpClient GraphQLHttpClient { get; }

        internal Uri BaseUri { get; }
        private readonly Uri _graphQl = new Uri("graph/api/graphql", UriKind.Relative);

        internal readonly bool _verifySsl;
        internal readonly WebProxy _proxy;

        private IndicoStrawberryShakeClient _indicoStrawberryShakeClient;

        internal IndicoStrawberryShakeClient IndicoStrawberryShakeClient =>
            _indicoStrawberryShakeClient
            ?? (_indicoStrawberryShakeClient = new IndicoStrawberryShakeClient(BaseUri, _graphQl, _apiToken, _verifySsl, _proxy));

        /// <summary>
        /// Creates IndicoClient for <inheritdoc cref="_defaultUrl"/>
        /// </summary>
        /// <param name="apiToken">Authentication token (You can generate one at <c>https://app.indico.io/auth/account</c>)</param>
        public IndicoClient(string apiToken, bool verify = true) : this(apiToken, new Uri(_defaultUrl), verify)
        { }

        /// <summary>
        /// Creates IndicoClient
        /// </summary>
        /// <param name="apiToken">Authentication token (You can generate one at <c>https://app.indico.io/auth/account</c>)</param>
        /// <param name="baseUri">indico.io base address (Default values is <c>https://app.indico.io</c>)</param>
        /// <param name="verify">verify the host's SSL certificate (Default value is <c>true</c>)</param>
        public IndicoClient(string apiToken, Uri baseUri, bool verify = true, WebProxy proxy = null)
        {
            _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
            BaseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            _verifySsl = verify;
            _proxy = proxy;
            var handler = new AuthenticatingMessageHandler(baseUri, apiToken, proxy);
            HttpClient = new HttpClient(handler);
            var options = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri($"{baseUri}/graph/api/graphql"),
                HttpMessageHandler = handler
            };
            GraphQLHttpClient = new GraphQLHttpClient(options, new NewtonsoftJsonSerializer(), HttpClient);
        }

        /// <summary>
        /// Create a new GraphQL request
        /// </summary>
        /// <param name="query">The GraphQL query or mutation</param>
        /// <param name="variables">variables defined in query or mutation</param>
        /// <returns>GraphQLRequest</returns>
        public GraphQLRequestClient GraphQLRequest()
        {
            var request = new GraphQLRequestClient(GraphQLHttpClient);
            return request;
        }
    }
}
