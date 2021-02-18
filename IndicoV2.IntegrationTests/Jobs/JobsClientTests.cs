using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.Jobs;
using IndicoV2.Submissions;
using IndicoV2.Submissions.Models;
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

        [Test]
        public async Task GenerateSubmissionResultAsync_ShouldReturnJob()
        {
            // Arrange
            var submissionId = (await _dataHelper.Submissions().GetAnyAsync()).Id;
            while (SubmissionStatus.PROCESSING == (await _submissionsClient.GetAsync(submissionId)).Status)
            {
                await Task.Delay(50);
            }

            // Act
            var jobId = await _jobsClient.GenerateSubmissionResultAsync(submissionId);

            // Assert
            jobId.Should().NotBeEmpty();
        }
    }
}
