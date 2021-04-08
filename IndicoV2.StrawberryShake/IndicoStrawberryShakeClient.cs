using System;
using IndicoV2.StrawberryShake.DataSets;
using IndicoV2.StrawberryShake.DataSets.Wrappers;
using IndicoV2.StrawberryShake.HttpClient;
using Microsoft.Extensions.DependencyInjection;

namespace IndicoV2.StrawberryShake
{
    public class IndicoStrawberryShakeClient
    {
        private readonly ServiceProvider _services;

        public IndicoStrawberryShakeClient(Uri baseUri, Uri graphQlEndpoint, string token)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddSingleton(new AuthenticatingMessageHandler(baseUri, token))
                .AddIndicoGqlClient()
                .ConfigureHttpClient(
                    (sp, c) => c.BaseAddress = new Uri(baseUri, graphQlEndpoint),
                    builder => builder.ConfigurePrimaryHttpMessageHandler<AuthenticatingMessageHandler>());

            serviceCollection.AddSingleton<IAddFilesClient, AddFilesClient>();

            _services = serviceCollection.BuildServiceProvider();
        }

        public DataSetClientGql DataSets() => new DataSetClientGql(_services);
    }
}
