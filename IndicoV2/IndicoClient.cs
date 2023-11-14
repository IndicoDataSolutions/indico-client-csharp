using System;
using Indico;
using IndicoV2.StrawberryShake;

namespace IndicoV2
{
    /// <summary>
    /// Client for the indico.io platform.
    /// </summary>
    public class IndicoClient : IIndicoClient
    {
        /// <summary>https://try.indico.io</summary>
        private const string _defaultUrl = "https://try.indico.io";

        internal readonly string _apiToken;

        private Indico.IndicoClient _legacyClient;

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
        /// <param name="apiToken">Authentication token (You can generate one at <c>https://try.indico.io/auth/account</c>)</param>
        public IndicoClient(string apiToken, bool verify = true) : this(apiToken, new Uri(_defaultUrl), verify)
        { }

        /// <summary>
        /// Creates IndicoClient
        /// </summary>
        /// <param name="apiToken">Authentication token (You can generate one at <c>https://try.indico.io/auth/account</c>)</param>
        /// <param name="baseUri">indico.io base address (Default values is <c>https://try.indico.io</c>)</param>
        /// <param name="verify">verify the host's SSL certificate (Default value is <c>true</c>)</param>
        public IndicoClient(string apiToken, Uri baseUri, bool verify = true)
        {
            _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
            BaseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            _verifySsl = verify;
        }
    }
}
