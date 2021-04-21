using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IndicoV2.DataSets;
using IndicoV2.Extensions.DataSets;
using IndicoV2.StrawberryShake;
using IndicoV2.Tests.Automock;
using Moq;
using NUnit.Framework;

namespace IndicoV2.Tests.Extensions.DataSets
{
    public class DataSetAwaiterTests
    {
        private IndicoAutoMockingFixture _fixture;

        [SetUp]
        public void SetUp() => _fixture = new IndicoAutoMockingFixture();

        private static readonly FileStatus[] _downloadedOrFailed = {FileStatus.Downloaded, FileStatus.Failed};

        private static readonly FileStatus[] _notDownloadedOrFailed =
            Enum.GetValues(typeof(FileStatus)).Cast<FileStatus>()
                .Except(_downloadedOrFailed)
                .ToArray();

        [Test]
        public async Task WaitFilesDownloadedOrFailedAsync_ShouldWait_UntilAllFinished(
            [ValueSource(nameof(_notDownloadedOrFailed))] FileStatus notDownloadedOrFailed, 
            [ValueSource(nameof(_downloadedOrFailed))]FileStatus downloadedOrFailed)
        {
            // Arrange
            const int dataSetId = 2;
            var cancellationToken = new CancellationToken();
            var dataSetClientGqlMock = _fixture.Freeze<Mock<IDataSetClient>>();
            dataSetClientGqlMock
                .SetupSequence(cli => cli.FileUploadStatusAsync(dataSetId, cancellationToken))
                .ReturnsAsync(BuildDataSetUploadStatusResult(notDownloadedOrFailed))
                .ReturnsAsync(BuildDataSetUploadStatusResult(downloadedOrFailed));
            var awaiter = _fixture.Create<DataSetAwaiter>();

            // Act
            await awaiter.WaitFilesDownloadedOrFailedAsync(dataSetId, TimeSpan.Zero, cancellationToken);

            // Assert
            dataSetClientGqlMock
                .Verify(cli => cli.FileUploadStatusAsync(dataSetId, cancellationToken), Times.Exactly(2));
        }

        [Test]
        public void WaitFilesDownloadedOrFailedAsync_ShouldThrow_AfterTokenCancelled()
        {
            // Arrange
            _fixture.Freeze<Mock<IDataSetClient>>()
                .Setup(cli => cli.FileUploadStatusAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BuildDataSetUploadStatusResult(_notDownloadedOrFailed.First()));
            var awaiter = _fixture.Create<DataSetAwaiter>();

            // Act, Assert
            awaiter.Invoking(async aw =>
                    await aw.WaitFilesDownloadedOrFailedAsync(default, default, new CancellationToken(true)))
                .Should()
                .Throw<TaskCanceledException>();
        }

        private static readonly FileStatus[] _processedOrFailed = { FileStatus.Processed, FileStatus.Failed };

        private static readonly FileStatus[] _notProcessedOrFailed =
            Enum.GetValues(typeof(FileStatus)).Cast<FileStatus>()
                .Except(_processedOrFailed)
                .ToArray();

        [Test]
        public async Task WaitFilesProcessedOrFailedAsync_ShouldWait_UntilAllFinished(
            [ValueSource(nameof(_notProcessedOrFailed))] FileStatus notDownloadedOrFailed,
            [ValueSource(nameof(_processedOrFailed))] FileStatus downloadedOrFailed)
        {
            // Arrange
            const int dataSetId = 2;
            var cancellationToken = new CancellationToken();
            var dataSetClientGqlMock = _fixture.Freeze<Mock<IDataSetClient>>();
            dataSetClientGqlMock
                .SetupSequence(cli => cli.FileUploadStatusAsync(dataSetId, cancellationToken))
                .ReturnsAsync(BuildDataSetUploadStatusResult(notDownloadedOrFailed))
                .ReturnsAsync(BuildDataSetUploadStatusResult(downloadedOrFailed));
            var awaiter = _fixture.Create<DataSetAwaiter>();

            // Act
            await awaiter.WaitFilesProcessedOrFailedAsync(dataSetId, TimeSpan.Zero, cancellationToken);

            // Assert
            dataSetClientGqlMock
                .Verify(cli => cli.FileUploadStatusAsync(dataSetId, cancellationToken), Times.Exactly(2));
        }

        [Test]
        public void WaitFilesProcessedOrFailedAsync_ShouldThrow_AfterTokenCancelled()
        {
            // Arrange
            _fixture.Freeze<Mock<IDataSetClient>>()
                .Setup(cli => cli.FileUploadStatusAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BuildDataSetUploadStatusResult(_notDownloadedOrFailed.First()));
            var awaiter = _fixture.Create<DataSetAwaiter>();

            // Act, Assert
            awaiter.Invoking(async aw =>
                    await aw.WaitFilesProcessedOrFailedAsync(default, default, new CancellationToken(true)))
                .Should()
                .Throw<TaskCanceledException>();
        }

        public IDatasetUploadStatusResult BuildDataSetUploadStatusResult(FileStatus status)
        {
            var file = Mock.Of<IDatasetUploadStatus_Dataset_Files>(f => f.Status == status);
            var dataSet = Mock.Of<IDatasetUploadStatus_Dataset>(ds => ds.Files == new[] {file});
            var response = Mock.Of<IDatasetUploadStatusResult>(r => r.Dataset == dataSet);

            return response;
        }
    }
}
