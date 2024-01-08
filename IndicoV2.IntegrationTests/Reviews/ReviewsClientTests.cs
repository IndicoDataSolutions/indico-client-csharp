using System;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.Extensions.SubmissionResult;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.IntegrationTests.Utils.Configs;
using IndicoV2.Reviews;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Unity;
using IndicoV2.Jobs;

namespace IndicoV2.IntegrationTests.Reviews
{
    public class ReviewsClientTests
    {
        private IReviewsClient _reviewsClient;
        private DataHelper _dataHelper;
        private ISubmissionResultAwaiter _submissionResultAwaiter;
        private IJobsClient _jobsClient;
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
            _jobsClient = container.Resolve<IJobsClient>();
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
        public async Task SubmitReviewAsync_ShouldSucceed()
        {
            // Arrange
            var submission = await _dataHelper.Submissions().GetAnyAsync(_workflowId);
            var result = await _submissionResultAwaiter.WaitReady(submission.Id);
            var changes = (JObject)result["results"]["document"]["results"];

            // Act
            var submitReviewJobId = await _reviewsClient.SubmitReviewAsync(submission.Id, changes);
            var jobResult = JObject.Parse(await _jobsClient.GetResultAsync(submitReviewJobId));

            // Assert
            jobResult.Should().NotBeNull();
            jobResult.Value<string>("submission_status").Should().Be("pending_review");
            jobResult.Value<bool>("success").Should().Be(true);
        }
    }
}
