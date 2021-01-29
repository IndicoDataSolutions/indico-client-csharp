using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.Reviews;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.Reviews
{
    public class ReviewsClientTests
    {
        private IReviewsClient _reviewsClient;
        private DataHelper _dataHelper;

        [SetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            _dataHelper = container.Resolve<DataHelper>();
            _reviewsClient = container.Resolve<IReviewsClient>();
        }

        [Test]
        public async Task RejectAsync_ShouldReturnJobId()
        {
            // Arrange
            var submissionId = await GetSubmissionIdWithAutoReview();
            
            // Act
            var jobId = await _reviewsClient.RejectAsync(submissionId, default);

            // Assert
            jobId.Should().NotBeNullOrEmpty();
            Assert.Inconclusive("Requires JobsClient to verify job's result");
        }

        [Test]
        public async Task SubmitReviewAsync_ShouldReturnJobId()
        {
            // Arrange
            var submission = await GetSubmissionIdWithAutoReview();

            // Act
            var jobId = await _reviewsClient.SubmitReviewAsync(submission, JObject.Parse("{}"), default);

            // Assert
            jobId.Should().NotBeNullOrEmpty();
            Assert.Inconclusive("Requires JobsClient to verify job's result");
        }

        private async Task<int> GetSubmissionIdWithAutoReview() =>
            await _dataHelper.Submissions().Get(
                await _dataHelper.Workflows().Get(true), 
                new MemoryStream(new byte[3]));
    }
}
