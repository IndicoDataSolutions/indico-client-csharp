using System;
using Indico;
using IndicoV2.Abstractions;

namespace IndicoV2
{
    public class IndicoClient : IIndicoClient
    {
        private readonly Uri _baseUri;
        private Indico.IndicoClient _legacyClient;

        internal Indico.IndicoClient LegacyClient =>
            _legacyClient ?? (_legacyClient = new Indico.IndicoClient(new IndicoConfig(host: _baseUri.Host)));

        public IndicoClient(Uri baseUri)
        {
            _baseUri = baseUri;
        }
    }
}
