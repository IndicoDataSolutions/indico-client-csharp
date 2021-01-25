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
        public async Task CreateAsync_ShouldCreateSubmission_FromStream()
        {
            // Arrange
            var workflow = await _dataHelper.Workflows().GetAnyWorkflow();
            await using var fileStream = await _dataHelper.Files().GetSampleFileStream();

            // Act
            var submissionIds = await _submissionsClient.CreateAsync(workflow.Id, new[] { fileStream });

            // Assert
            var submissionId = submissionIds.Single();
            submissionId.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task CreateAsync_ShouldCreateSubmission_FromFilePath()
        {
            // Arrange
            var workflow = await _dataHelper.Workflows().GetAnyWorkflow();
            var filePath = _dataHelper.Files().GetSampleFilePath();

            // Act
            var submissionIds = await _submissionsClient.CreateAsync(workflow.Id, new[] { filePath });

            // Assert
            var submissionId = submissionIds.Single();
            submissionId.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task GetAsync_ShouldFetchSubmission()
        {
            // Arrange
            var submissionId = (await _dataHelper.Submissions().GetAnyAsync()).Id;

            // Act
            var submission = await _submissionsClient.GetAsync(submissionId);

            // Assert
            submission.Id.Should().BeGreaterThan(0);
            submission.Status.Should().BeOfType<SubmissionStatus>();
        }
    }
}
