using System;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.Extensions.Jobs;
using IndicoV2.Extensions.SubmissionResult;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.IntegrationTests.Utils.Configs;
using IndicoV2.Reviews;
using IndicoV2.Submissions.Models;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Unity;
using System.Text.Json;
using IndicoV2.Submissions;
using System.Linq;
using System.Collections.Generic;
using IndicoV2.Reviews.Models;

namespace IndicoV2.IntegrationTests.Reviews
{
    public class ReviewsClientTests
    {
        private IReviewsClient _reviewsClient;
        private DataHelper _dataHelper;
        private ISubmissionResultAwaiter _submissionResultAwaiter;
        private JobAwaiter _jobAwaiter;
        private IndicoConfigs _indicoConfigs;
        private int _workflowId;

        [SetUp]
        public async Task SetUp()
        {
            var container = new IndicoTestContainerBuilder()
                .ForAutoReviewWorkflow()
                .Build();
            _dataHelper = container.Resolve<DataHelper>();
            _reviewsClient = container.Resolve<IReviewsClient>();
            _submissionResultAwaiter = container.Resolve<ISubmissionResultAwaiter>();
            _jobAwaiter = container.Resolve<JobAwaiter>();
            _indicoConfigs = new IndicoConfigs();
            var _rawWorkflowId = _indicoConfigs.WorkflowId;
            if (_rawWorkflowId == 0)
            {
                var _workflow = await _dataHelper.Workflows().GetAnyWorkflow();
                _workflowId = _workflow.Id;
            }
            else
            {
                _workflowId = _rawWorkflowId;
            }
        }

        [Test]
        public async Task SubmitReviewAsyncV1Result_ShouldSucceed()
        {
            // Arrange
            var submission = await _dataHelper.Submissions().Get(_workflowId, resultsFileVersion: SubmissionResultsFileVersion.One);
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


        [Test]
        public async Task SubmitReviewAsyncV3Result_ShouldSucceed()
        {
            // Arrange
            var submission = await _dataHelper.Submissions().Get(_workflowId, resultsFileVersion: SubmissionResultsFileVersion.Three);
            var result = await _submissionResultAwaiter.WaitReady(submission.Id);
            submission = (await _dataHelper.Submissions().ListAsync(submissionIds: new List<int>() { submission.Id })).First();
            if (submission.Status.ToString() == "PENDING_AUTO_REVIEW")
            {
                var changes = result["submission_results"];
                foreach (JObject change in changes)
                {
                    change["model_results"] = change["model_results"]["ORIGINAL"];
                    change["component_results"] = change["component_results"]["ORIGINAL"];
                }
                // Act
                var submitReviewJobId = await _reviewsClient.SubmitReviewAsync(submission.Id, (JArray)changes);
                var jobResult = await _jobAwaiter.WaitReadyAsync<JObject>(submitReviewJobId, TimeSpan.FromSeconds(1));

                // Assert
                jobResult.Should().NotBeNull();
                jobResult.Value<string>("submission_status").Should().Be("pending_review");
                jobResult.Value<bool>("success").Should().Be(true);
            }
            else
            {
                // multi-file submissions are not enabled and submission status should default to complete
                submission.Status.ToString().Should().Be("COMPLETE");
            }
        }


        [Test]
        public async Task GetReviewsAsync_ShouldSucceed()
        {
            var submission = await _dataHelper.Submissions().Get(_workflowId);
            var reviews = await _reviewsClient.GetReviewsAsync(submission.Id);

            foreach (var review in reviews)
            {
                review.Id.Should().NotBeNull();
                review.Changes.Should().BeOfType<JObject>();
            }
        }
    }
}
