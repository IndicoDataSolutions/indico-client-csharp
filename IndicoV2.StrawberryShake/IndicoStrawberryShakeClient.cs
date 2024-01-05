using System;
using IndicoV2.StrawberryShake.DataSets;
using IndicoV2.StrawberryShake.HttpClient;
using IndicoV2.StrawberryShake.Reporting;
using IndicoV2.StrawberryShake.Submissions;
using IndicoV2.StrawberryShake.Workflows;
using IndicoV2.StrawberryShake.Models;
using Microsoft.Extensions.DependencyInjection;

namespace IndicoV2.StrawberryShake
{
    public class IndicoStrawberryShakeClient
    {
        private readonly ServiceProvider _services;

        public IndicoStrawberryShakeClient(Uri baseUri, Uri graphQlEndpoint, string token, bool verify)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddSingleton(new AuthenticatingMessageHandler(baseUri, token))
                .AddIndicoGqlClient()
                .ConfigureHttpClient(
                    (sp, c) => c.BaseAddress = new Uri(baseUri, graphQlEndpoint),
                    builder => builder.ConfigurePrimaryHttpMessageHandler<AuthenticatingMessageHandler>());

            serviceCollection.AddSingleton<DataSetSsClient>();

            _services = serviceCollection.BuildServiceProvider();
        }

        public DataSetSsClient DataSets() => new DataSetSsClient(_services);
        public ModelSsClient Models() => new ModelSsClient(_services);
        public WorkflowSsClient Workflows() => new WorkflowSsClient(_services);
        public SubmissionSsClient Submissions() => new SubmissionSsClient(_services);
        public UserReportingSsClient UserReporting() => new UserReportingSsClient(_services);

    }
}
