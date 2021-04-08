using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IndicoV2.DataSets;
using IndicoV2.Extensions.DataSets;
using IndicoV2.StrawberryShake.DataSets;
using IndicoV2.StrawberryShake.IndicoGqlClient;
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

        private static readonly FileStatus[] _fileStatusesFinished =
        {
            FileStatus.Processed, FileStatus.Downloaded, FileStatus.Failed
        };

        private static readonly FileStatus[] _fileStatusesUnfinished =
            Enum.GetValues(typeof(FileStatus)).Cast<FileStatus>()
                .Except(_fileStatusesFinished)
                .ToArray();

        [Test]
        public async Task WaitAllFilesFinished_ShouldWait_UntilAllFinished(
            [ValueSource(nameof(_fileStatusesUnfinished))] FileStatus statusUnfinished, 
            [ValueSource(nameof(_fileStatusesFinished))]FileStatus statusFinished)
        {
            // Arrange
            const int dataSetId = 2;
            var cancellationToken = new CancellationToken();
            var dataSetClientGqlMock = _fixture.Freeze<Mock<IDataSetClient>>();
            dataSetClientGqlMock
                .SetupSequence(cli => cli.FileUploadStatus(dataSetId, cancellationToken))
                .ReturnsAsync(BuildDataSetUploadStatusResult(statusUnfinished))
                .ReturnsAsync(BuildDataSetUploadStatusResult(statusFinished));
            var awaiter = _fixture.Create<DataSetAwaiter>();

            // Act
            await awaiter.WaitAllFilesProcessedAsync(dataSetId, TimeSpan.Zero, cancellationToken);

            // Assert
            dataSetClientGqlMock
                .Verify(cli => cli.FileUploadStatus(dataSetId, cancellationToken), Times.Exactly(2));
        }

        [Test]
        public async Task WaitAllFilesFinished_ShouldThrow_AfterTokenCancelled()
        {
            // Arrange
            _fixture.Freeze<Mock<IDataSetClient>>()
                .Setup(cli => cli.FileUploadStatus(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BuildDataSetUploadStatusResult(_fileStatusesUnfinished.First()));
            var awaiter = _fixture.Create<DataSetAwaiter>();

            // Act, Assert
            awaiter.Invoking(async aw =>
                    await aw.WaitAllFilesProcessedAsync(default, default, new CancellationToken(true)))
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
