using System;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.Extensions.SubmissionResult;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.IntegrationTests.Utils.Configs;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.Extensions.SubmissionResult
{
    public class SubmissionResultAwaiterTests
    {
        private SubmissionResultAwaiter _sut;
        private DataHelper _dataHelper;
        private IndicoConfigs _indicoConfigs;
        private int _workflowId;

        [SetUp]
        public async Task SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();

            _sut = container.Resolve<SubmissionResultAwaiter>();
            _dataHelper = container.Resolve<DataHelper>();
            _indicoConfigs = new IndicoConfigs();
            var _rawWorkflowId = _indicoConfigs.WorkflowId;
            if (_rawWorkflowId == 0)
            {
                var _workflow = await _dataHelper.Workflows().GetAnyWorkflow();
                _workflowId = _workflow.Id;
            }
            else
            {
                _workflowId = _rawWorkflowId;
            }
        }

        [Test]
        public async Task WaitReady_ShouldReturnJobResult()
        {
            // Arrange
            var submissionId = (await _dataHelper.Submissions().GetAnyAsync(_workflowId)).Id;

            // Act
            var jobResult = await _sut.WaitReady(submissionId, TimeSpan.FromMilliseconds(300), default);

            // Assert
            jobResult.Should().NotBeNull();
            jobResult["results"]["document"].Should().NotBeNull();
        }
    }
}
