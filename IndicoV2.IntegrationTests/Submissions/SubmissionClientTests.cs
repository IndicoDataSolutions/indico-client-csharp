using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.Submissions;
using IndicoV2.Submissions.Models;

using NUnit.Framework;

using Unity;

namespace IndicoV2.IntegrationTests.Submissions
{
    public class SubmissionClientTests
    {
        private DataHelper _dataHelper;
        private ISubmissionsClient _submissionsClient;

        [SetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            _submissionsClient = container.Resolve<ISubmissionsClient>();
            _dataHelper = container.Resolve<DataHelper>();
        }

        [Test]
        public async Task GetAsync_ShouldFetchSubmission()
        {
            // Arrange
            var submissionId = (await _dataHelper.Submissions().GetAnyAsync(new MemoryStream(new byte[5]))).Id;

            // Act
            var submission = await _submissionsClient.GetAsync(submissionId);

            // Assert
            submission.Id.Should().BeGreaterThan(0);
            submission.Status.Should().BeOfType<SubmissionStatus>();
        }

        [Test]
        public async Task ListAsync_ShouldFetchSubmissions()
        {
            // Arrange
            var listData = (await _dataHelper.Submissions().ListAnyAsync(new MemoryStream(new byte[5])));

            // Act
            var submissions = await _submissionsClient.ListAsync(new List<int> { listData.submissionId }, new List<int> { listData.workflowId }, null);
            var submission = submissions.First();

            // Assert
            submissions.Should().HaveCountGreaterThan(0);
            submission.Id.Should().BeGreaterThan(0);
            submission.Status.Should().BeOfType<SubmissionStatus>();
        }
    }
}
