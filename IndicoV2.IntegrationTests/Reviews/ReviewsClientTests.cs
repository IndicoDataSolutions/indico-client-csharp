using System;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.Extensions.Jobs;
using IndicoV2.Extensions.SubmissionResult;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.Jobs.Exceptions;
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
        public async Task SubmitReviewAsync_ShouldReturnJob()
        {
            // Arrange
            var submission = await _dataHelper.Submissions().GetAnyAsync();
            var result = await _submissionResultAwaiter.WaitReady(submission.Id);
            var changes = (JObject)result["results"]["document"]["results"];

            // as in: https://indicodatasolutions.github.io/indico-client-python/auto-review.html?highlight=submitreview
            foreach (var (_, predictions) in changes)
            {
                foreach (var prediction in predictions.Value<JArray>("pre_review"))
                {
                    var label = prediction.Value<string>("label");
                    if (prediction["confidence"].Value<double>(label) < 0.6d)
                    {
                        prediction["rejected"] = true;
                    }
                }
            }

            // Act
            var jobId = await _reviewsClient.SubmitReviewAsync(submission.Id, changes);
            try
            {
                var jobResult = await _jobAwaiter.WaitReadyAsync(jobId, TimeSpan.FromSeconds(1));
                
                // Assert
                jobResult.Should().NotBeNullOrEmpty();
            }
            catch (JobNotSuccessfulException)
            {
                // TODO: fix the review
                // Jobs's errorr: "Extraction predictions must be a list"
                Assert.Fail("TODO: investigate what's wrong");
            }

            Assert.Inconclusive("Requires JobsClient to verify job's result");
        }
    }
}
