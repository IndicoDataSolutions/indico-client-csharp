using System;
using Indico;

namespace IndicoV2
{
    /// <summary>
    /// Client for the indico.io platform.
    /// </summary>
    public class IndicoClient : IIndicoClient
    {
        private readonly Uri _baseUri;
        private readonly string _apiToken;
        private Indico.IndicoClient _legacyClient;

        internal Indico.IndicoClient LegacyClient =>
            _legacyClient ?? (_legacyClient = new Indico.IndicoClient(new IndicoConfig(host: _baseUri.Host, apiToken:_apiToken)));

        /// <summary>
        /// Creates IndicoClient
        /// </summary>
        /// <param name="apiToken">Authentication token (You can generate one at "<see cref="baseUri"/>/auth/account" )</param>
        /// <param name="baseUri">indico.io base addres (Default values is "https://app.indico.io")</param>
        public IndicoClient(string apiToken, Uri baseUri = default)
        {
            _baseUri = baseUri ?? new Uri("https://app.indico.io");
            _apiToken = apiToken;
        }
    }
}
