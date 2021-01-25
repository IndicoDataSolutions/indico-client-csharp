using System;
using Indico;
using IndicoV2.DataSets;
using IndicoV2.Submissions;
using IndicoV2.Workflows;
using Unity;
using V1Client = Indico.IndicoClient;
using V2Client = IndicoV2.IndicoClient;

namespace IndicoV2.IntegrationTests.Utils
{
    internal class IndicoTestContainerBuilder
    {
        private string BaseUrl => Environment.GetEnvironmentVariable("INDICO_HOST");
        private string ApiToken => Environment.GetEnvironmentVariable("INDICO_TOKEN");

        public IUnityContainer Build()
        {
            var container = new UnityContainer();
            container.RegisterFactory<V1Client>(c => new V1Client(new IndicoConfig(
                ApiToken,
                host: new Uri(BaseUrl).Host)));
            container.RegisterFactory<V2Client>(c => new V2Client(ApiToken, new Uri(BaseUrl)));
            container.RegisterType<IIndicoClient, V2Client>();
            container.RegisterFactory<IDataSetClient>(c => c.Resolve<V2Client>().DataSets());
            container.RegisterFactory<IWorkflowsClient>(c => c.Resolve<V2Client>().Workflows());
            container.RegisterFactory<ISubmissionsClient>(c => c.Resolve<V2Client>().Submissions());

            return container;
        }
    }
}
