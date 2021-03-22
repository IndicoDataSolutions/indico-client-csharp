using System;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.Extensions.Jobs;
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
        private ISubmissionResultAwaiter _submissionResultAwaiter;
        private JobAwaiter _jobAwaiter;

        [SetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder()
                .ForAutoReviewWorkflow()
                .Build();
            _dataHelper = container.Resolve<DataHelper>();
            _reviewsClient = container.Resolve<IReviewsClient>();
            _submissionResultAwaiter = container.Resolve<ISubmissionResultAwaiter>();
            _jobAwaiter = container.Resolve<JobAwaiter>();
        }

        [Test]
        public async Task SubmitReviewAsync_ShouldSucceed()
        {
            // Arrange
            var submission = await _dataHelper.Submissions().GetAnyAsync();
            var result = await _submissionResultAwaiter.WaitReady(submission.Id);
            var changes = (JObject)result["results"]["document"]["results"];

            // Act
            var submitReviewJobId = await _reviewsClient.SubmitReviewAsync(submission.Id, changes);
            var jobResult = await _jobAwaiter.WaitReadyAsync<JObject>(submitReviewJobId, TimeSpan.FromSeconds(1));

            // Assert
            jobResult.Should().NotBeNull();
            jobResult.Value<string>("submission_status").Should().Be("pending_review");
            jobResult.Value<bool>("success").Should().Be(true);
        }
    }
}
