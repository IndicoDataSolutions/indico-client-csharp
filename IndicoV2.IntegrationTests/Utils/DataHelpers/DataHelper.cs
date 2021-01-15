using IndicoV2.IntegrationTests.Utils.DataHelpers.Submissions;
using Unity;

namespace IndicoV2.IntegrationTests.Utils.DataHelpers
{
    internal class DataHelper
    {
        private readonly IUnityContainer _container;

        public DataHelper(IUnityContainer container)
        {
            _container = container;
        }

        public SubmissionHelper Submissions() => _container.Resolve<SubmissionHelper>();
    }
}
