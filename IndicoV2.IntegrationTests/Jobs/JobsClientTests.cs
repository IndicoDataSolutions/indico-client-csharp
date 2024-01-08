using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.IntegrationTests.Utils.Configs;
using IndicoV2.Jobs;
using IndicoV2.Jobs.Models;
using IndicoV2.Submissions;
using IndicoV2.Submissions.Models;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.Jobs
{
    public class JobsClientTests
    {
        private DataHelper _dataHelper;
        private IJobsClient _jobsClient;
        private ISubmissionsClient _submissionsClient;
        private IndicoConfigs _indicoConfigs;
        private int _workflowId;

        [SetUp]
        public async Task SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            _jobsClient = container.Resolve<IJobsClient>();
            _submissionsClient = container.Resolve<ISubmissionsClient>();
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
        public async Task GetStatusAsync_ShouldReturnJobStatus() => (await _jobsClient.GetStatusAsync(await GetAnyJobIdAsync())).Should().BeOfType<JobStatus>();

        [Test]
        public async Task GetResultAsync_ShouldReturnJobResult()
        {
            // Arrange
            var jobId = await GetAnyJobIdAsync();

            // Act
            var jobResult = JToken.Parse(await _jobsClient.GetResultAsync(jobId));

            // Assert
            jobResult.Should().NotBeNull();
            jobResult["url"].Should().NotBeNull();
        }

        private async Task<string> GetAnyJobIdAsync()
        {
            var submission = await _dataHelper.Submissions().GetAnyAsync(_workflowId);
            while ((await _submissionsClient.GetAsync(submission.Id)).Status == SubmissionStatus.PROCESSING)
            {
                await Task.Delay(100);
            }

            var jobId = await _submissionsClient.GenerateSubmissionResultAsync(submission.Id);

            return jobId;
        }
    }
}
