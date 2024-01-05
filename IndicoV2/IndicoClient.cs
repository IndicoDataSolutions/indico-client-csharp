﻿using System;
using System.Net.Http;
using Indico;
using IndicoV2.StrawberryShake;
using IndicoV2.StrawberryShake.HttpClient;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

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

        private Indico.IndicoClient _legacyClient;

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

        internal Indico.IndicoClient LegacyClient =>
            _legacyClient ?? (_legacyClient =
                new Indico.IndicoClient(new IndicoConfig(host: BaseUri.Host, port: BaseUri.Port, apiToken: _apiToken, verify: _verifySsl)));

        private IndicoStrawberryShakeClient _indicoStrawberryShakeClient;

        internal IndicoStrawberryShakeClient IndicoStrawberryShakeClient =>
            _indicoStrawberryShakeClient
            ?? (_indicoStrawberryShakeClient = new IndicoStrawberryShakeClient(BaseUri, _graphQl, _apiToken, _verifySsl));

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
        public IndicoClient(string apiToken, Uri baseUri, bool verify = true)
        {
            _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
            BaseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            _verifySsl = verify;
            var handler = new AuthenticatingMessageHandler(baseUri, apiToken);
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
        /// <returns>GraphQLRequest</returns>
        public GraphQLRequest.GraphQLRequest GraphQLRequest(string query=null, string operationName=null, object variables = null)
        {
            var request = new GraphQLRequest.GraphQLRequest(GraphQLHttpClient);
            if (query != null)
            {
                request.Query = query;
            }

            if (operationName != null)
            {
                request.OperationName = operationName;
            }

            if (variables != null)
            {
                request.Variables = variables;
            }
            return request;
        }
    }
}
