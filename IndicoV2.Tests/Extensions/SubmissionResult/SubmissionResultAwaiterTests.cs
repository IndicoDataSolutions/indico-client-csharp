using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IndicoV2.Extensions.Jobs;
using IndicoV2.Extensions.SubmissionResult;
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
        private static readonly SubmissionStatus[] _submissionStatusesExceptProcessing =
            Enum.GetValues(typeof(SubmissionStatus)).Cast<SubmissionStatus>().Where(s => s != SubmissionStatus.PROCESSING).ToArray();
        private IFixture _fixture;

        [SetUp]
        public void CreateAutoMockFixture() => _fixture = new IndicoAutoMockingFixture();

        [TestCaseSource(nameof(_submissionStatusesExceptProcessing))]
        public async Task WaitReady_ShouldReturnJobResult_WhenCorrectStatuses(
            [ValueSource(nameof(_submissionStatusesExceptProcessing))]
            SubmissionStatus status)
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
                .Setup(cli => cli.GetAsync(It.IsAny<Uri>()))
                .ReturnsJsonStream("{}");

            var sut = _fixture.Create<SubmissionResultAwaiter>();

            // Act
            var result = await sut.WaitReady(submissionId, checkInterval, default);

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
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.FAILED));
            submissionClientMock
                .Setup(cli => cli.GenerateSubmissionResultAsync(submissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);
            _fixture.Freeze<Mock<IJobAwaiter>>()
                .Setup(cli => cli.WaitReadyAsync(jobId, checkInterval, It.IsAny<CancellationToken>()))
                .ReturnsAsync(JObject.Parse(@"{""url"": ""test""}"));
            _fixture.Freeze<Mock<IStorageClient>>()
                .Setup(cli => cli.GetAsync(It.IsAny<Uri>()))
                .ReturnsJsonStream("{}");

            var sut = _fixture.Create<SubmissionResultAwaiter>();

            // Act
            await sut.WaitReady(submissionId, checkInterval, default);

            // Assert
            submissionClientMock.Verify(cli => cli.GetAsync(submissionId, It.IsAny<CancellationToken>()),
                Times.Exactly(2));
            submissionClientMock.Verify(cli => cli.GenerateSubmissionResultAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            submissionClientMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task WaitReady_ShouldReturnJsonObject()
        {
            // Arrange
            const int submissionId = 1;
            const string jobId = "testJobId";
            var checkInterval = TimeSpan.Zero;
            var submissionClientMock = _fixture.Freeze<Mock<ISubmissionsClient>>();
            var storageUri = new Uri("https://test");
            submissionClientMock
                .Setup(cli => cli.GetAsync(submissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.FAILED));
            submissionClientMock
                .Setup(j => j.GenerateSubmissionResultAsync(submissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);
            _fixture.Freeze<Mock<IJobAwaiter>>()
                .Setup(cli => cli.WaitReadyAsync(jobId, checkInterval, It.IsAny<CancellationToken>()))
                .ReturnsAsync(JObject.Parse($@"{{ ""url"": ""{storageUri}"" }}"));
            _fixture.Freeze<Mock<IStorageClient>>()
                .Setup(cli => cli.GetAsync(storageUri))
                .ReturnsJsonStream(@"{ ""test"" : 13 }");
            var sut = _fixture.Create<SubmissionResultAwaiter>();

            // Act
            var result = await sut.WaitReady(submissionId, default, default);

            // Asesrt
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
                .Returns(async () => Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.PROCESSING));
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
                .Setup(cli => cli.GetAsync(It.IsAny<Uri>()))
                .ReturnsJsonStream("{}");

            var sut = _fixture.Create<SubmissionResultAwaiter>();
            var waitForStatus = statusChanges.Last();

            // Act
            await sut.WaitReady(submissionId, waitForStatus, TimeSpan.Zero, default);

            // Assert
            submissionsClientMock.Verify(cli => cli.GetAsync(submissionId, It.IsAny<CancellationToken>()), Times.Exactly(statusChanges.Length));
            submissionsClientMock.Verify(cli => cli.GenerateSubmissionResultAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
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
    }
}
