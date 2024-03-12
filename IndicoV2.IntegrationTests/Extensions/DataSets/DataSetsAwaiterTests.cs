using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.DataSets;
using IndicoV2.Extensions.DataSets;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.StrawberryShake;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.Extensions.DataSets
{
    public class DataSetsAwaiterTests
    {
        private IDataSetAwaiter _dataSetAwaiter;
        private IDataSetClient _dataSetsClient;
        private DataHelper _dataHelper;

        [SetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            var client = container.Resolve<IndicoClient>();
            _dataSetAwaiter = client.DataSetAwaiter();
            _dataSetsClient = client.DataSets();
            _dataHelper = container.Resolve<DataHelper>();
        }

        [Test]
        public async Task WaitAllFilesDownloadedOrFailedAsync_ShouldWaitUntilFilesDownloadedOrFailed()
        {
            // Arrange
            var dataSet = await _dataHelper.DataSets().GetAny();
            var files = new[] {_dataHelper.Files().GetSampleFilePath()};
            await _dataSetsClient.AddFilesAsync(dataSet.Id, files, default);

            // Act
            await _dataSetAwaiter.WaitFilesDownloadedOrFailedAsync(dataSet.Id, TimeSpan.FromSeconds(0.5), default);

            // Assert
            var dataSetWithStatuses = await _dataSetsClient.FileUploadStatusAsync(dataSet.Id, default);
            dataSetWithStatuses.Dataset.Files
                .Select(f => f.Status)
                .All(s => s is FileStatus.Downloaded or FileStatus.Failed or FileStatus.Processed)
                .Should().BeTrue();
        }

        [Test]
        public async Task WaitFilesProcessedOrFailedAsync_ShouldWaitUntilAllFilesProcessedOrFailed()
        {
            // Arrange
            var dataSet = await _dataHelper.DataSets().GetAny();
            var filePaths = new[] {_dataHelper.Files().GetSampleFilePath()};
            await _dataSetsClient.AddFilesAsync(dataSet.Id, filePaths, default);
            var container = new IndicoTestContainerBuilder().Build();
            var client = container.Resolve<IndicoClient>();
            _dataSetAwaiter = client.DataSetAwaiter();
            await _dataSetAwaiter.WaitFilesDownloadedOrFailedAsync(dataSet.Id, TimeSpan.Zero, default);

            var dataSetFileStatus = await _dataSetsClient.FileUploadStatusAsync(dataSet.Id, default);
            var downloadedFileIds = dataSetFileStatus.Dataset.Files.Where(f => f.Status == FileStatus.Downloaded).Select(f => f.Id.Value);

            await _dataSetsClient.ProcessFileAsync(dataSet.Id, downloadedFileIds, default);

            // Act
            container = new IndicoTestContainerBuilder().Build();
            client = container.Resolve<IndicoClient>();
            _dataSetAwaiter = client.DataSetAwaiter();
            await _dataSetAwaiter.WaitFilesProcessedOrFailedAsync(dataSet.Id, TimeSpan.Zero, default);

            // Assert
            var dataSetWithStatuses = await _dataSetsClient.FileUploadStatusAsync(dataSet.Id, default);
            dataSetWithStatuses.Dataset.Files
                .All(f => f.Status is FileStatus.Processed or FileStatus.Failed)
                .Should().BeTrue();
        }
    }
}
