using System;
using Indico;

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

        internal Uri BaseUri { get; }
        internal Indico.IndicoClient LegacyClient =>
            _legacyClient ?? (_legacyClient =
                new Indico.IndicoClient(new IndicoConfig(host: BaseUri.Host, apiToken: _apiToken)));

        /// <summary>
        /// Creates IndicoClient for <inheritdoc cref="_defaultUrl"/>
        /// </summary>
        /// <param name="apiToken">Authentication token (You can generate one at "<see cref="baseUri"/>/auth/account" )</param>
        public IndicoClient(string apiToken): this(apiToken, new Uri(_defaultUrl))
        { }

        /// <summary>
        /// Creates IndicoClient
        /// </summary>
        /// <param name="apiToken">Authentication token (You can generate one at "<see cref="baseUri"/>/auth/account" )</param>
        /// <param name="baseUri">indico.io base addres (Default values is "https://app.indico.io")</param>
        public IndicoClient(string apiToken, Uri baseUri)
        {
            _apiToken = apiToken  ?? throw new ArgumentNullException(nameof(apiToken));
            BaseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
        }
    }
}
