using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.Jobs;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.Jobs
{
    public class JobsClientTests
    {
        private DataHelper _dataHelper;
        private IJobsClient _jobsClient;

        [SetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            _dataHelper = container.Resolve<DataHelper>();
            _jobsClient = container.Resolve<IJobsClient>();
        }

        [Test]
        public async Task GenerateSubmissionResultAsync_ShouldReturnJob()
        {
            // Arrange
            var submissionId = (await _dataHelper.Submissions().GetAnyAsync()).Id;

            // Act
            var job = await _jobsClient.GenerateSubmissionResultAsync(submissionId);

            // Assert
            job.Id.Should().BeGreaterThan(0);
        }
    }
}
