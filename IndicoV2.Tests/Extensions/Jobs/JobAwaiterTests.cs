using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IndicoV2.Extensions.Jobs;
using IndicoV2.Jobs;
using IndicoV2.Jobs.Exceptions;
using IndicoV2.Jobs.Models;
using IndicoV2.Tests.Automock;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace IndicoV2.Tests.Extensions.Jobs
{
    public class JobAwaiterTests
    {
        private IndicoAutoMockingFixture _fixture;

        [SetUp]
        public void SetUp() => _fixture = new IndicoAutoMockingFixture();

        private static readonly JobStatus[] _waitingForResultStatuses = { JobStatus.PENDING, JobStatus.RECEIVED, JobStatus.STARTED };

        private static readonly JobStatus[] _finishedNotSuccessfulStatuses = Enum.GetValues(typeof(JobStatus))
            .Cast<JobStatus>()
            .Except(_waitingForResultStatuses.Union(new[] { JobStatus.SUCCESS }))
            .ToArray();

        [TestCaseSource(nameof(_waitingForResultStatuses))]
        public async Task WaitReadyAsync_ShouldWait_UntilJobProcessed(JobStatus inProgressStatus)
        {
            // Arrange
            var jobId = Guid.NewGuid().ToString();
            var jobsClientMock = _fixture.Freeze<Mock<IJobsClient>>();
            jobsClientMock
                .SetupSequence(j => j.GetStatusAsync(jobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(inProgressStatus)
                .ReturnsAsync(JobStatus.SUCCESS);
            jobsClientMock
                .Setup(cli => cli.GetResultAsync<JObject>(jobId, default))
                .ReturnsAsync(JObject.Parse(@"{ ""test"": ""test"" }"));
            var sut = _fixture.Create<JobAwaiter>();

            // Act
            await sut.WaitReadyAsync<JObject>(jobId, TimeSpan.Zero, default);

            // Assert
            jobsClientMock.Verify(j => j.GetStatusAsync(jobId, It.IsAny<CancellationToken>()), Times.Exactly(2));
            jobsClientMock.Verify(j => j.GetResultAsync<JObject>(jobId, default), Times.Once);
            jobsClientMock.VerifyNoOtherCalls();
        }

        [TestCaseSource(nameof(_finishedNotSuccessfulStatuses))]
        public void WaitReadyAsync_ShouldThrow_WhenFinishedNotSuccessful(JobStatus jobStatus)
        {
            // Arrange
            const string jobId = "testJobId";
            _fixture.Freeze<Mock<IJobsClient>>()
                .Setup(cli => cli.GetStatusAsync(jobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobStatus);
            var jobAwaiter = _fixture.Create<JobAwaiter>();

            // Act, Assert
            this.Invoking(_ => jobAwaiter.WaitReadyAsync(jobId, default, new CancellationTokenSource(500).Token))
                .Should().Throw<JobNotSuccessfulException>();
        }

        [Test]
        public void WaitReadyAsync_ShouldThrow_WhenCancelled()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();
            _fixture.Freeze<Mock<IJobsClient>>()
                .Setup(cli => cli.GetStatusAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(JobStatus.PENDING);
            var jobAwaiter = _fixture.Create<JobAwaiter>();

            // Act
            var action = jobAwaiter.WaitReadyAsync("test", TimeSpan.FromSeconds(1), tokenSource.Token);
            tokenSource.Cancel();

            // Assert
            this.Invoking(async _ => await action).Should().Throw<TaskCanceledException>();
        }
    }
}
