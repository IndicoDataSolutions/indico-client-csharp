using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.Submissions;
using IndicoV2.Submissions.Models;
using IndicoV2.Workflows.Models;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.Submissions
{
    public class SubmissionClientTests
    {
        private DataHelper _dataHelper;
        private ISubmissionsClient _submissionsClient;
        private IWorkflow _workflow;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            _submissionsClient = (SubmissionsClient)container.Resolve<ISubmissionsClient>();
            _dataHelper = container.Resolve<DataHelper>();

            _workflow = await _dataHelper.Workflows().GetAnyWorkflow();
        }

        [Test]
        public async Task CreateAsync_ShouldCreateSubmission_FromStream()
        {
            // Arrange
            await using var fileStream = _dataHelper.Files().GetSampleFileStream();

            // Act

            var submissionIds = await _submissionsClient.CreateAsync(_workflow.Id, new List<(string f, Stream c)> { ("csharp_test_content", fileStream) });

            // Assert
            var submissionId = submissionIds.Single();
            submissionId.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task CreateAsync_ShouldCreateSubmission_FromStreamWithName()
        {
            // Arrange
            await using var fileStream = _dataHelper.Files().GetSampleFileStream();
            var filePath = _dataHelper.Files().GetSampleFilePath();

            // Act
            var submissionIds = await _submissionsClient.CreateAsync(_workflow.Id, new[] { (filePath, fileStream) });

            // Assert
            var submissionId = submissionIds.Single();
            submissionId.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task CreateAsync_ShouldCreateSubmission_FromFilePath()
        {
            // Arrange
            var filePath = _dataHelper.Files().GetSampleFilePath();

            // Act
            var submissionIds = await _submissionsClient.CreateAsync(_workflow.Id, new[] { filePath });

            // Assert
            var submissionId = submissionIds.Single();
            submissionId.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task CreateAsync_ShouldCreateSubmission_FromUri()
        {
            // Arrange
            var uri = _dataHelper.Uris().GetSampleUri();

            // Act
            var submissionIds = await _submissionsClient.CreateAsync(_workflow.Id, new[] { uri });

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
        }

        [Test]
        public async Task ListAsync_ShouldFetchSubmissions()
        {
            // Arrange
            var listData = await _dataHelper.Submissions().ListAnyAsync();

            // Act
            var submissions = await _submissionsClient.ListAsync(new List<int> { listData.submissionId }, new List<int> { listData.workflowId }, null);
            var submission = submissions.First();

            // Assert
            submissions.Should().HaveCountGreaterThan(0);
            submission.Id.Should().BeGreaterThan(0);
            submission.Status.Should().BeOfType<SubmissionStatus>();
        }


        [Test]
        public async Task ListAsync_ShouldFetchSubmissionsWithCursor()
        {
            // Arrange
            var listData = await _dataHelper.Submissions().ListAnyAsync();

            // Act
            var submissions = await _submissionsClient.ListAsync(new List<int> { listData.submissionId }, new List<int> { listData.workflowId }, null, 0, 1000);

            submissions.Should().NotBeNull();
            submissions.PageInfo.Should().NotBeNull();
            submissions.Data.Should().NotBeNull();
            var submission = submissions.Data.First();

            // Assert
            submissions.Data.Should().HaveCountGreaterThan(0);
            submission.Id.Should().BeGreaterThan(0);
            submission.Status.Should().BeOfType<SubmissionStatus>();
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
            var jobId = await _submissionsClient.GenerateSubmissionResultAsync(submissionId);

            // Assert
            jobId.Should().NotBeEmpty();
        }

        [Test]
        public async Task MarkSubmissionAsRetrieved_ShouldUpdateSubmission()
        {
            // Arrange
            var submissionId = (await _dataHelper.Submissions().GetAnyAsync()).Id;
            while (SubmissionStatus.PROCESSING == (await _submissionsClient.GetAsync(submissionId)).Status)
            {
                await Task.Delay(50);
            }

            // Act
            var submission = await _submissionsClient.MarkSubmissionAsRetrieved(submissionId);
            // Assert
            submission.Should().NotBeNull();

            var result = await _submissionsClient.ListAsync(new List<int>() { submission.Id }, new List<int>(), null);
            result.Should().NotBeNullOrEmpty();

            var updated_sub = result.First();
            updated_sub.Should().NotBeNull();
            updated_sub.Retrieved.Should().BeTrue();

        }
    }
}
