using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IndicoV2.Submissions;
using IndicoV2.Submissions.LongRunning;
using IndicoV2.Submissions.Models;
using IndicoV2.Tests.Automock;
using Moq;
using NUnit.Framework;


namespace IndicoV2.Tests.Submissions.LongRunning
{
    public class SubmissionAwaiterTests
    {
        private IFixture _fixture;

        [SetUp]
        public void CreateAutoMockFixture() => _fixture = new IndicoAutoMockingFixture();

        [TestCase(SubmissionStatus.COMPLETE)]
        [TestCase(SubmissionStatus.FAILED)]
        [TestCase(SubmissionStatus.PENDING_ADMIN_REVIEW)]
        [TestCase(SubmissionStatus.PENDING_REVIEW)]
        public async Task WaitReady_ShouldReturnJob_WhenNotPending(SubmissionStatus status)
        {
            // Arrange
            const int submissionId = 1;
            var submissionClientMock = _fixture.Freeze<Mock<ISubmissionsClient>>();
            submissionClientMock
                .Setup(cli => cli.GetAsync(submissionId, default))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == status));
            var expectedResult = Mock.Of<IJob>();
            submissionClientMock.Setup(cli => cli.GetJobAsync(submissionId, default)).ReturnsAsync(expectedResult);
            var sut = _fixture.Create<SubmissionAwaiter>();

            // Act
            var result = await sut.WaitReady(submissionId, TimeSpan.Zero, TimeSpan.Zero, default);

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public async Task WaitReady_ShouldNotReturn_WhenProcessing()
        {
            // Arrange
            const int submissionId = 1;
            
            var submissionClientMock = _fixture.Freeze<Mock<ISubmissionsClient>>();
            submissionClientMock
                .SetupSequence(cli => cli.GetAsync(submissionId, default))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.PROCESSING))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.FAILED));
            var sut = _fixture.Create<SubmissionAwaiter>();

            // Act
            var result = await sut.WaitReady(submissionId, TimeSpan.Zero, TimeSpan.FromSeconds(1), default);

            // Assert
            submissionClientMock.Verify(cli => cli.GetAsync(submissionId, default), Times.Exactly(2));
            submissionClientMock.Verify(cli => cli.GenerateSubmissionResult(submissionId, default), Times.Once);
            submissionClientMock.VerifyNoOtherCalls();
        }

        [Test]
        public void WaitReady_ThrowsWhenTimeout()
        {
            // Arrange
            const int submissionId = 1;
            _fixture.Freeze<Mock<ISubmissionsClient>>().Setup(cli => cli.GetAsync(submissionId, default))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.PROCESSING));
            var sut = _fixture.Create<SubmissionAwaiter>();

            // Act, Assert
            var ex = Assert.ThrowsAsync<TimeoutException>(async () =>
                await sut.WaitReady(submissionId, TimeSpan.Zero, TimeSpan.Zero, default));
            ex.Message.Should().Be("Timeout exceeded (00:00:00).");
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task WaitReady_ShouldPassCancellationTokenToAllMethods(bool isCancelled)
        {
            // Arrange
            const int submissionId = 1;
            var cancellationToken = new CancellationToken(isCancelled);
            var submissionClientMock = _fixture.Freeze<Mock<ISubmissionsClient>>();
            submissionClientMock
                .Setup(cli => cli.GetAsync(submissionId, cancellationToken))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.COMPLETE));
            var sut = _fixture.Create<SubmissionAwaiter>();
            
            // Act
            await sut.WaitReady(submissionId, TimeSpan.Zero, TimeSpan.Zero, cancellationToken);

            // Assert
            submissionClientMock.Verify(cli => cli.GetAsync(submissionId, It.IsNotNull<CancellationToken>()), Times.Exactly(1));
            submissionClientMock.Verify(cli => cli.GenerateSubmissionResult(submissionId, It.IsNotNull<CancellationToken>()), Times.Exactly(1));
            submissionClientMock.VerifyNoOtherCalls();
        }

        [Test]
        public void WaitReady_ShouldThrowWhenCancelled()
        {
            // Arrange
            _fixture.Freeze<Mock<ISubmissionsClient>>()
                .Setup(cli => cli.GetAsync(It.IsAny<int>(), default))
                .ReturnsAsync(Mock.Of<ISubmission>(s => s.Status == SubmissionStatus.PROCESSING));
            var sut = _fixture.Create<SubmissionAwaiter>();

            // Act, Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () =>
                await sut.WaitReady(default, default, TimeSpan.MaxValue, new CancellationToken(true)));
        }
    }
}
