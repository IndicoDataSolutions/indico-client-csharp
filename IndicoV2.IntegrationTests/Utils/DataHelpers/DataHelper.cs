using IndicoV2.IntegrationTests.Utils.DataHelpers.Files;
using IndicoV2.IntegrationTests.Utils.DataHelpers.Submissions;
using IndicoV2.IntegrationTests.Utils.DataHelpers.Uris;
using IndicoV2.IntegrationTests.Utils.DataHelpers.Workflows;
using Unity;

namespace IndicoV2.IntegrationTests.Utils.DataHelpers
{
    internal class DataHelper
    {
        private readonly IUnityContainer _container;

        public DataHelper(IUnityContainer container) => _container = container;

        public SubmissionHelper Submissions() => _container.Resolve<SubmissionHelper>();
        
        public WorkflowHelper Workflows() => _container.Resolve<WorkflowHelper>();

        public FileHelper Files() => _container.Resolve<FileHelper>();

        public UriHelper Uris() => _container.Resolve<UriHelper>();

    }
}
