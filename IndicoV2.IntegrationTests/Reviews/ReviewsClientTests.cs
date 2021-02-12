using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.Extensions.SubmissionResult;
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
            var container = new IndicoTestContainerBuilder().ForAutoReviewWorkflow().Build();
            _dataHelper = container.Resolve<DataHelper>();
            _reviewsClient = container.Resolve<IReviewsClient>();
            container.Resolve<SubmissionResultAwaiter>()
        }

        [Test]
        public async Task SubmitReviewAsync_ShouldReturnJobId()
        {
            // Arrange
            var submission = await _dataHelper.Submissions().GetAnyAsync();
            

            // Act
            var jobId = await _reviewsClient.SubmitReviewAsync(submission.Id, JObject.Parse("{}"), default);

            // Assert
            jobId.Should().NotBeNullOrEmpty();
            Assert.Inconclusive("Requires JobsClient to verify job's result");
        }
    }
}
