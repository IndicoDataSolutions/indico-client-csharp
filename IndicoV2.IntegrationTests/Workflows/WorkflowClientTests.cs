using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.DataSets;
using IndicoV2.IntegrationTests.Utils;
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
        private IWorkflowsClient _workflowsClient;

        [SetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            _dataSetClient = container.Resolve<IDataSetClient>();
            _workflowsClient = container.Resolve<IWorkflowsClient>();
            _dataHelper = container.Resolve<DataHelper>();
        }

        [Test]
        public async Task AddData_ShouldReturnResult()
        {
            var dataSet = await _dataHelper.DataSets().GetAny();
            await _dataSetClient.AddFilesAsync(dataSet.Id, new[] {_dataHelper.Files().GetSampleFilePath()}, default);
            var workflows = await _workflowsClient.ListAsync(dataSet.Id, default);

            var result = await _workflowsClient.AddDataAsync(workflows.First().Id, default);

            result.Should().NotBeNull();
        }
    }
}
