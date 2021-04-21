using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.DataSets;
using IndicoV2.Extensions.Workflows;
using IndicoV2.IntegrationTests.Utils;
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

        [SetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            var client = container.Resolve<IndicoClient>();
            _dataSetsClient = client.DataSets();
            _workflowsClient = client.Workflows();
            _workflowAwaiter = client.WorkflowAwaiter();
            _dataHelper = container.Resolve<DataHelper>();
        }

        [Test]
        public async Task WaitComplete_ShouldWaitForTheStatus()
        {
            var dataSet = await _dataHelper.DataSets().GetAny();
            var workflows = await _workflowsClient.ListAsync(dataSet.Id, default);
            var workflow = workflows.First();

            await _dataSetsClient.AddFilesAsync(dataSet.Id, new[] {_dataHelper.Files().GetSampleFilePath()}, default);
            await _workflowsClient.AddDataAsync(workflow.Id, default);

            await _workflowAwaiter.WaitWorkflowCompleteAsync(workflow.Id, TimeSpan.FromSeconds(0.5), default);

            (await _workflowsClient.GetStatusAsync(workflow.Id, default)).Should().Be(WorkflowStatus.Complete);
        }
    }
}
