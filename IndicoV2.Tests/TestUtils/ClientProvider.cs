using Indico;
using IndicoV2.Client.Submissions;
using IndicoV2.Tests.Client;
using IndicoV2.Tests.TestUtils.System.Net;
using IndicoV2.V1Adapters.Client.Submissions;

namespace IndicoV2.Tests.TestUtils
{
    internal class ClientProvider
    {
        public ClientProvider()
        {
            Handler = new FakeHttpMessageHandler();
            V1Client = new IndicoTestClient(
                new IndicoConfig(apiToken: "TestToken"),
                Handler);
        }

        public FakeHttpMessageHandler Handler { get; }

        public IndicoClient V1Client { get; }

        internal ISubmissionsClient SubmissionsClient => new IndicoSubmissionsAdapter(V1Client);
    }
}
