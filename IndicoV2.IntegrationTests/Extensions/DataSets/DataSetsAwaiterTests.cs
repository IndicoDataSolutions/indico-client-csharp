using System;
using System.Linq;
using System.Threading.Tasks;
using IndicoV2.DataSets;
using IndicoV2.Extensions.DataSets;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.StrawberryShake.IndicoGqlClient;
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
        public async Task WaitAllFilesProcessedAsync_ShouldWaitUntilFilesProcessed()
        {
            // Arrange
            var dataSet = await _dataHelper.DataSets().GetAny();
            var files = new[] {_dataHelper.Files().GetSampleFilePath()};
            await _dataSetsClient.AddFilesAsync(dataSet.Id, files, default);

            // Act
            await _dataSetAwaiter.WaitAllFilesProcessedAsync(dataSet.Id, TimeSpan.FromSeconds(0.5), default);

            // Assert
            var dataSetWithStatuses = await _dataSetsClient.FileUploadStatus(dataSet.Id, default);
            dataSetWithStatuses.Dataset.Files.Select(f => f.Status).All(s =>
                s == FileStatus.Downloaded || s == FileStatus.Failed || s == FileStatus.Processed);
        }
    }
}
