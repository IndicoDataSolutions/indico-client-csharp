using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.DataSets;
using IndicoV2.Extensions.Workflows;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.Configs;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.StrawberryShake;
using IndicoV2.Workflows;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.Extensions.Workflows
{
    public class WorkflowAwaiterTests
    {
        private IDataSetClient _dataSetsClient;
        private DataHelper _dataHelper;
        private IWorkflowsClient _workflowsClient;
        private WorkflowAwaiter _workflowAwaiter;
        private IndicoConfigs _indicoConfigs;
        private int _workflowId;
        private int _dataSetId;

        [SetUp]
        public async Task SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            var client = container.Resolve<IndicoClient>();
            _dataSetsClient = client.DataSets();
            _workflowsClient = client.Workflows();
            _workflowAwaiter = client.WorkflowAwaiter();
            _dataHelper = container.Resolve<DataHelper>();
            _indicoConfigs = new IndicoConfigs();
            var _rawDataSetId = _indicoConfigs.DatasetId;
            if (_rawDataSetId == 0)
            {
                var dataset = (await _dataHelper.DataSets().GetAny());
                _dataSetId = dataset.Id;
                var workflows = await _workflowsClient.ListAsync(_dataSetId, default);
                _workflowId = workflows.First().Id;
            }
            else
            {
                _dataSetId = _rawDataSetId;
                int _rawWorkflowID = _indicoConfigs.WorkflowId;
                if (_rawWorkflowID == 0)
                {
                    var workflows = await _workflowsClient.ListAsync(_dataSetId, default);
                    _workflowId = workflows.First().Id;
                }
                else
                {
                    _workflowId = _rawWorkflowID;
                }
            }

        }

        [Test]
        public async Task WaitComplete_ShouldWaitForTheStatus()
        {

            await _dataSetsClient.AddFilesAsync(_dataSetId, new[] {_dataHelper.Files().GetSampleFilePath()}, default);
            await _workflowsClient.AddDataAsync(_workflowId, default);

            await _workflowAwaiter.WaitWorkflowCompleteAsync(_workflowId, TimeSpan.FromSeconds(0.5), default);

            (await _workflowsClient.GetStatusAsync(_workflowId, default)).Should().Be(WorkflowStatus.Complete);
        }
    }
}
