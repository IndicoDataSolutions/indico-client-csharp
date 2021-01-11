using Indico;
using IndicoV2.Tests.TestUtils.System.Net;

namespace IndicoV2.Tests.Client
{
    internal class IndicoTestClient : IndicoClient
    {
        public IndicoTestClient(IndicoConfig indicoConfig, FakeHttpMessageHandler handler) : base(indicoConfig, handler)
        {
        }
    }
}