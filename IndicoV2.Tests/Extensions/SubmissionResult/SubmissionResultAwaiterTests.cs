using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IndicoV2.Extensions.Jobs;
using IndicoV2.Extensions.SubmissionResult;
using IndicoV2.Extensions.SubmissionResult.Exceptions;
using IndicoV2.Storage;
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
        private static readonly SubmissionStatus[] _submissionStatusesExceptProcessingAndFailed =
            Enum.GetValues(typeof(SubmissionStatus)).Cast<SubmissionStatus>().Where(s => s != SubmissionStatus.PROCESSING && s != SubmissionStatus.FAILED).ToArray();
        private IFixture _fixture;

        [SetUp]
        public void CreateAutoMockFixture() => _fixture = new IndicoAutoMockingFixture();

        [TestCaseSource(nameof(_submissionStatusesExceptProcessingAndFailed))]
     
        public async Task WaitReady_ShouldReturnJobResult_WhenCorrectStatuses(SubmissionStatus status)
        {
            // Arrange
            const int submissionId = 1;
            var jobId = Guid.NewGuid().ToString();
            var checkInterval = TimeSpan.Zero;
            _fixture.Freeze<Mock<ISubmissionsClient>>()
                .Setup(cli => cli.GetAsync(submissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == status));
            _fixture.Freeze<Mock<ISubmissionsClient>>()
                .Setup(cli => cli.GenerateSubmissionResultAsync(submissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);
            _fixture.Freeze<Mock<IJobAwaiter>>()
                .Setup(cli => cli.WaitReadyAsync(jobId, checkInterval, It.IsAny<CancellationToken>()))
                .ReturnsAsync(JObject.Parse(@"{""url"": ""test"" }"));
            _fixture.Freeze<Mock<IStorageClient>>()
                .Setup(cli => cli.GetAsync(It.IsAny<Uri>(), default))
                .ReturnsJsonStream("{}");

            var sut = _fixture.Create<SubmissionResultAwaiter>();

            // Act
            var result = await sut.WaitReady(submissionId, status, checkInterval, default);

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
            var checkInterval = TimeSpan.Zero;
            submissionClientMock
                .SetupSequence(cli => cli.GetAsync(submissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.PROCESSING))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.COMPLETE));
            submissionClientMock
                .Setup(cli => cli.GenerateSubmissionResultAsync(submissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);
            _fixture.Freeze<Mock<IStorageClient>>()
                .Setup(cli => cli.GetAsync(It.IsAny<Uri>(), default))
                .ReturnsJsonStream("{}");

            var sut = _fixture.Create<SubmissionResultAwaiter>();

            // Act
            await sut.WaitReady(submissionId, checkInterval, default);

            // Assert
            submissionClientMock.Verify(cli => cli.GetAsync(submissionId, It.IsAny<CancellationToken>()), Times.Exactly(2));
            submissionClientMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task WaitReady_ShouldReturnJsonObject()
        {
            // Arrange
            const int submissionId = 1;
            const string jobId = "testJobId";
            var submissionClientMock = _fixture.Freeze<Mock<ISubmissionsClient>>();
            const string resultFile = "/test/resultFile.x";
            var storageUri = new Uri($"indico-file://{resultFile}");
            submissionClientMock
                .Setup(cli => cli.GetAsync(submissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.COMPLETE && s.ResultFile == resultFile));
            submissionClientMock
                .Setup(j => j.GenerateSubmissionResultAsync(submissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);
            _fixture.Freeze<Mock<IStorageClient>>()
                .Setup(cli => cli.GetAsync(storageUri, default))
                .ReturnsJsonStream(@"{ ""test"" : 13 }");
            var sut = _fixture.Create<SubmissionResultAwaiter>();

            // Act
            var result = await sut.WaitReady(submissionId, default, default);

            // Assert
            result.Value<int>("test").Should().Be(13);
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
                await sut.WaitReady(default, default, new CancellationToken(true)));
        }

        [Test]
        public void WaitReady_ShouldThrow_WhenCancelled()
        {
            // Arrange
            _fixture
                .Freeze<Mock<ISubmissionsClient>>()
                .Setup(cli => cli.GetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.PROCESSING)));
            var sut = _fixture.Create<SubmissionResultAwaiter>();
            var cancellationTokenSource = new CancellationTokenSource();
            var runTask = sut.WaitReady(1, TimeSpan.FromMilliseconds(1), cancellationTokenSource.Token);

            // Act
            cancellationTokenSource.Cancel();

            // Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () => await runTask);
        }

        [TestCase(SubmissionStatus.PENDING_ADMIN_REVIEW, SubmissionStatus.PENDING_REVIEW, SubmissionStatus.PENDING_AUTO_REVIEW)]
        public async Task Task_WaitReadyWithCustomStatus_ShouldWait_UntilSubmissionReachesStatus(params SubmissionStatus[] statusChanges)
        {
            // Arrange
            const int submissionId = 1;
            var submissionsClientMock = _fixture.Freeze<Mock<ISubmissionsClient>>();
            var getSubmissionSequenceSetup = submissionsClientMock.SetupSequence(cli => cli.GetAsync(submissionId, It.IsAny<CancellationToken>()));
            
            foreach (var status in statusChanges)
            {
                getSubmissionSequenceSetup.ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == status));
            }

            _fixture.Freeze<Mock<IJobAwaiter>>()
                .Setup(cli =>
                    cli.WaitReadyAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(JObject.Parse(@"{ ""url"": ""test""}"));
            _fixture.Freeze<Mock<IStorageClient>>()
                .Setup(cli => cli.GetAsync(It.IsAny<Uri>(), default))
                .ReturnsJsonStream("{}");

            var sut = _fixture.Create<SubmissionResultAwaiter>();
            var waitForStatus = statusChanges.Last();

            // Act
            await sut.WaitReady(submissionId, waitForStatus, TimeSpan.Zero, default);

            // Assert
            submissionsClientMock.Verify(cli => cli.GetAsync(submissionId, It.IsAny<CancellationToken>()), Times.Exactly(statusChanges.Length));
            submissionsClientMock.VerifyNoOtherCalls();
        }

        [TestCase(SubmissionStatus.PENDING_REVIEW, SubmissionStatus.COMPLETE)]
        public void Task_WaitReadyWithCustomStatus_ShouldThrow_IfCustomStatusNotReached(SubmissionStatus statusReturned, SubmissionStatus statusAwaited)
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource(50).Token;
            const int submissionId = 1;
            _fixture.Freeze<Mock<ISubmissionsClient>>()
                .Setup(cli => cli.GetAsync(submissionId, cancellationToken))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == statusReturned));
            var sut = _fixture.Create<SubmissionResultAwaiter>();

            // Act
            var waitReady = sut.WaitReady(submissionId, statusAwaited, TimeSpan.FromMilliseconds(1), cancellationToken);

            // Asert
            this.Invoking(async _ => await waitReady)
                .Should()
                .Throw<TaskCanceledException>();
        }

        [TestCase(SubmissionStatus.PROCESSING)]
        [TestCase(SubmissionStatus.FAILED)]
        public void Task_WaitReadyWithCustomStatus_ShouldThrow_IfCustomStatusIsWrong(SubmissionStatus statusAwaited)
        {
            // Arrange
            const int submissionId = 1;
            var sut = _fixture.Create<SubmissionResultAwaiter>();

            // Act
            var waitReady = sut.WaitReady(submissionId, statusAwaited, TimeSpan.FromMilliseconds(1), default);

            // Asert
            this.Invoking(async _ => await waitReady)
                .Should()
                .Throw<ArgumentException>();
        }

        [Test]
        public void Task_WaitReadyWithCustomStatus_ShouldThrow_IfReturnedStatusIsFailed()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource(50).Token;
            const int submissionId = 1;
            _fixture.Freeze<Mock<ISubmissionsClient>>()
                .Setup(cli => cli.GetAsync(submissionId, cancellationToken))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.FAILED));
            var sut = _fixture.Create<SubmissionResultAwaiter>();

            // Act
            var waitReady = sut.WaitReady(submissionId, SubmissionStatus.COMPLETE, TimeSpan.FromMilliseconds(1), cancellationToken);

            // Asert
            this.Invoking(async _ => await waitReady)
                .Should()
                .Throw<WrongSubmissionStatusException>();
        }
    }
}
