using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.DataSets;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.Configs;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.Workflows;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.Workflows
{
    public class WorkflowClientTests
    {
        private IDataSetClient _dataSetClient;
        private DataHelper _dataHelper;
        private IndicoConfigs _indicoConfigs;
        private IWorkflowsClient _workflowsClient;
        private int _workflowId;

        [SetUp]
        public async Task SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            _dataSetClient = container.Resolve<IDataSetClient>();
            _workflowsClient = container.Resolve<IWorkflowsClient>();
            _dataHelper = container.Resolve<DataHelper>();
            _indicoConfigs = new IndicoConfigs();
            int _rawWorkflowID = _indicoConfigs.WorkflowId;
            if (_rawWorkflowID == 0)
            {
                var workflow = (await _dataHelper.Workflows().GetAnyWorkflow());
                _workflowId = workflow.Id;
            }
            else
            {
                _workflowId = _rawWorkflowID;
            }
        }

        [Test]
        public async Task AddData_ShouldReturnResult()
        {
            var dataSetId = _indicoConfigs.DatasetId;
            await _dataSetClient.AddFilesAsync(dataSetId, new[] {_dataHelper.Files().GetSampleFilePath()}, default);
            var result = await _workflowsClient.AddDataAsync(_workflowId, default);

            result.Should().NotBeNull();
        }
    }
}
