using System;
using System.Net;
using IndicoV2.StrawberryShake.DataSets;
using IndicoV2.StrawberryShake.HttpClient;
using IndicoV2.StrawberryShake.Reporting;
using IndicoV2.StrawberryShake.Submissions;
using IndicoV2.StrawberryShake.Workflows;
using IndicoV2.StrawberryShake.Models;
using Microsoft.Extensions.DependencyInjection;
using IndicoV2.StrawberryShake.Jobs;
using IndicoV2.StrawberryShake.Reviews;
using IndicoV2.StrawberryShake.Ocr;
using System.Net.Http;
using System.Net.Security;

namespace IndicoV2.StrawberryShake
{
    public class IndicoStrawberryShakeClient
    {
        private readonly ServiceProvider _services;

        public IndicoStrawberryShakeClient(Uri baseUri, Uri graphQlEndpoint, string token, bool verify, WebProxy proxy = null, SocketsHttpHandler? socketHandler = null)
        {
            var serviceCollection = new ServiceCollection();
            if(proxy != null && socketHandler.UseProxy == false)
            {
                socketHandler.UseProxy = true;
                socketHandler.Proxy = proxy;
            }
            var sslOptions = new SslClientAuthenticationOptions
            {
                // Leave certs unvalidated for debugging
                RemoteCertificateValidationCallback = delegate { return true; },
            };
            if (!verify)
            {
                socketHandler.SslOptions = sslOptions;
            }
            serviceCollection
                .AddSingleton(new AuthenticatingMessageHandler(baseUri, token))
                .AddSingleton(socketHandler ?? new SocketsHttpHandler())
                .AddIndicoGqlClient()
                .ConfigureHttpClient(
                    (sp, c) => c.BaseAddress = new Uri(baseUri, graphQlEndpoint),
                    builder => { builder.ConfigurePrimaryHttpMessageHandler<SocketsHttpHandler>();
                        builder.AddHttpMessageHandler<AuthenticatingMessageHandler>();
                    }
                  )
            ;

            serviceCollection.AddSingleton<DataSetSsClient>();

            _services = serviceCollection.BuildServiceProvider();
        }

        public DataSetSsClient DataSets() => new DataSetSsClient(_services);
        public ModelSsClient Models() => new ModelSsClient(_services);
        public WorkflowSsClient Workflows() => new WorkflowSsClient(_services);
        public JobSsClient Jobs() => new JobSsClient(_services);
        public SubmissionSsClient Submissions() => new SubmissionSsClient(_services);
        public ReviewSsClient Reviews() => new ReviewSsClient(_services);
        public OcrSsClient Ocr() => new OcrSsClient(_services);
        public UserReportingSsClient UserReporting() => new UserReportingSsClient(_services);

    }
}
