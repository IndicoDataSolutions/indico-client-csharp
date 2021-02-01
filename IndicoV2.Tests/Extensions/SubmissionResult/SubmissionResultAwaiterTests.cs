using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IndicoV2.Extensions.SubmissionResult;
using IndicoV2.Jobs;
using IndicoV2.Jobs.Models;
using IndicoV2.Submissions;
using IndicoV2.Submissions.Models;
using IndicoV2.Tests.Automock;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace IndicoV2.Tests.Extensions.SubmissionResult
{
    public class SubmissionResultAwaiterTests
    {
        private static readonly SubmissionStatus[] _submissionStatusesExceptProcessing =
            Enum.GetValues(typeof(SubmissionStatus)).Cast<SubmissionStatus>().Where(s => s != SubmissionStatus.PROCESSING).ToArray();
        private static readonly JobStatus[] _jobStatusesExceptPending =
            Enum.GetValues(typeof(JobStatus)).Cast<JobStatus>().Where(s => s != JobStatus.PENDING).ToArray();
        private readonly TimeSpan _timeoutDefault = TimeSpan.FromSeconds(5);
        private readonly TimeSpan _timeoutMax = TimeSpan.FromHours(1);
        private IFixture _fixture;

        [SetUp]
        public void CreateAutoMockFixture() => _fixture = new IndicoAutoMockingFixture();

        [Test, Combinatorial]
        public async Task WaitReady_ShouldReturnJobResult_WhenCorrectStatuses(
            [ValueSource(nameof(_submissionStatusesExceptProcessing))]
            SubmissionStatus status,
            [ValueSource(nameof(_jobStatusesExceptPending))]
            JobStatus jobStatus)
        {
            // Arrange
            const int submissionId = 1;
            var jobId = Guid.NewGuid().ToString();
            _fixture.Freeze<Mock<ISubmissionsClient>>()
                .Setup(cli => cli.GetAsync(submissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == status));
            _fixture.Freeze<Mock<IJobsClient>>()
                .Setup(cli => cli.GenerateSubmissionResultAsync(submissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);
            var jobsClientMock = _fixture.Freeze<Mock<IJobsClient>>();
            jobsClientMock
                .Setup(cli => cli.GetStatusAsync(jobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobStatus);
            jobsClientMock.Setup(cli => cli.GetResultAsync(jobId)).ReturnsAsync(JObject.Parse(@"{""url"": ""test"" }"));

            var sut = _fixture.Create<SubmissionResultAwaiter>();

            // Act
            var result = await sut.WaitReady(submissionId, TimeSpan.Zero, _timeoutDefault, default);

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public async Task WaitReady_ShouldWait_UntilSubmissionProcessed()
        {
            // Arrange
            const int submissionId = 1;
            var jobId = Guid.NewGuid().ToString();
            var submissionClientMock = _fixture.Freeze<Mock<ISubmissionsClient>>();
            submissionClientMock
                .SetupSequence(cli => cli.GetAsync(submissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.PROCESSING))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.FAILED));

            var jobsClientMock = _fixture.Freeze<Mock<IJobsClient>>();
            jobsClientMock
                .Setup(cli => cli.GenerateSubmissionResultAsync(submissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);
            jobsClientMock
                .Setup(cli => cli.GetStatusAsync(jobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(JobStatus.SUCCESS);
            jobsClientMock.Setup(cli => cli.GetResultAsync(jobId)).ReturnsAsync(JObject.Parse(@"{""url"": ""test""}"));

            var sut = _fixture.Create<SubmissionResultAwaiter>();

            // Act
            await sut.WaitReady(submissionId, TimeSpan.Zero, _timeoutDefault, default);

            // Assert
            submissionClientMock.Verify(cli => cli.GetAsync(submissionId, It.IsAny<CancellationToken>()),
                Times.Exactly(2));
            submissionClientMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task WaitReady_ShouldWait_UntilJobProcessed()
        {
            // Arrange
            const int submissionId = 1;
            var jobId = Guid.NewGuid().ToString();
            var submissionClientMock = _fixture.Freeze<Mock<ISubmissionsClient>>();
            submissionClientMock
                .Setup(cli => cli.GetAsync(submissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.FAILED));

            var jobsClientMock = _fixture.Freeze<Mock<IJobsClient>>();
            jobsClientMock
                .Setup(j => j.GenerateSubmissionResultAsync(submissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);
            jobsClientMock
                .SetupSequence(j => j.GetStatusAsync(jobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(JobStatus.PENDING)
                .ReturnsAsync(JobStatus.FAILURE);
            jobsClientMock
                .Setup(cli => cli.GetResultAsync(jobId))
                .ReturnsAsync(JObject.Parse(@"{ ""url"": ""test"" }"));
            var sut = _fixture.Create<SubmissionResultAwaiter>();

            // Act
            await sut.WaitReady(submissionId, TimeSpan.Zero, _timeoutDefault, default);

            // Assert
            jobsClientMock.Verify(j => j.GenerateSubmissionResultAsync(submissionId, It.IsAny<CancellationToken>()), Times.Once);
            jobsClientMock.Verify(j => j.GetStatusAsync(jobId, It.IsAny<CancellationToken>()), Times.Exactly(2));
            jobsClientMock.Verify(j => j.GetResultAsync(jobId), Times.Once);
            jobsClientMock.VerifyNoOtherCalls();
        }

        [Test, Timeout(2000)]
        public void WaitReady_ShouldThrow_WhenTimeout()
        {
            // Arrange
            _fixture.Freeze<Mock<ISubmissionsClient>>()
                .Setup(cli => cli.GetAsync(It.IsAny<int>(), default))
                .Returns(async () =>
                {
                    await Task.Delay(_timeoutMax);
                    return null;
                });
            var sut = _fixture.Create<SubmissionResultAwaiter>();

            // Act
            Assert.ThrowsAsync<TaskCanceledException>(async () =>
                await sut.WaitReady(default, _timeoutMax, TimeSpan.Zero, default));
        }

        [Test]
        public void WaitReady_ShouldThrow_WhenReceivedCancelledToken()
        {
            // Arrange
            _fixture.Freeze<Mock<ISubmissionsClient>>()
                .Setup(cli => cli.GetAsync(It.IsAny<int>(), default))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.PROCESSING));
            var sut = _fixture.Create<SubmissionResultAwaiter>();

            // Act, Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () =>
                await sut.WaitReady(default, default, _timeoutMax, new CancellationToken(true)));
        }

        [Test]
        public void WaitReady_ShouldThrow_WhenCancelled()
        {
            // Arrange
            _fixture
                .Freeze<Mock<ISubmissionsClient>>()
                .Setup(cli => cli.GetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns(async () =>
                {
                    await Task.Delay(_timeoutMax);
                    throw new InvalidOperationException("Should not reach this part");
                });
            var sut = _fixture.Create<SubmissionResultAwaiter>();
            var cancellationTokenSource = new CancellationTokenSource();
            var runTask = sut.WaitReady(1, TimeSpan.Zero, _timeoutMax, cancellationTokenSource.Token);

            // Act
            cancellationTokenSource.Cancel();

            // Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () => await runTask);
        }
    }
}
