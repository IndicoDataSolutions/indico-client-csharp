using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
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

        [SetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            _jobsClient = container.Resolve<IJobsClient>();
            _submissionsClient = container.Resolve<ISubmissionsClient>();
            _dataHelper = container.Resolve<DataHelper>();
        }

        /*[Test]
        public async Task GetStatusAsync_ShouldReturnJobStatus() => (await _jobsClient.GetStatusAsync(await GetAnyJobIdAsync())).Should().BeOfType<JobStatus>();*/

        [Test]
        public async Task GetResultAsync_ShouldReturnJobResult()
        {
            // Arrange
            var jobId = await GetAnyJobIdAsync();

            // Act
            var jobResult = await _jobsClient.GetResultAsync<JToken>(jobId);

            // Assert
            jobResult.Should().NotBeNull();
            jobResult["url"].Should().NotBeNull();
        }

        private async Task<string> GetAnyJobIdAsync()
        {
            var submission = await _dataHelper.Submissions().GetAnyAsync();
            while ((await _submissionsClient.GetAsync(submission.Id)).Status == SubmissionStatus.PROCESSING)
            {
                await Task.Delay(100);
            }
            
            var jobId = await _submissionsClient.GenerateSubmissionResultAsync(submission.Id);

            return jobId;
        }
    }
}
