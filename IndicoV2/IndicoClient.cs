using System;
using Indico;

namespace IndicoV2
{
    public class IndicoClient : IIndicoClient
    {
        private readonly Uri _baseUri;
        private readonly string _apiToken;
        private Indico.IndicoClient _legacyClient;

        internal Indico.IndicoClient LegacyClient =>
            _legacyClient ?? (_legacyClient = new Indico.IndicoClient(new IndicoConfig(host: _baseUri.Host, apiToken:_apiToken)));

        public IndicoClient(Uri baseUri, string apiToken)
        {
            _baseUri = baseUri;
            _apiToken = apiToken;
        }
    }
}
