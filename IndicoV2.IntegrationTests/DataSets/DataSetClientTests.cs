using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.DataSets;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.StrawberryShake;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.DataSets
{
    /// <summary>
    /// There's no way to identify uploaded file (upload does not return Id, just metadata)
    /// so running those tests in parallel may cause problems (multiple tests using the same file)
    /// </summary>
    [Parallelizable(ParallelScope.None)]
    public class DataSetClientTests
    {
        private IDataSetClient _dataSetClient;
        private DataHelper _dataHelper;

        [SetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            _dataSetClient = container.Resolve<IDataSetClient>();
            _dataHelper = container.Resolve<DataHelper>();
        }

        [Test]
        public async Task ListAsync_ShouldReturnDataSets()
        {
            var dataSets = (await _dataSetClient.ListAsync()).ToArray();

            dataSets.Length.Should().BeGreaterThan(0);
            var ds = dataSets.First();
            ds.Id.Should().BeGreaterThan(0);
            ds.Name.Should().NotBeNullOrEmpty();
        }

        [TestCase(null)]
        [TestCase(1)]
        public async Task ListFullAsync_ShouldReturnDataSetsFull(int? limit)
        {
            var dataSets = (await _dataSetClient.ListFullAsync(limit)).ToArray();

            if (limit.HasValue)
            {
                dataSets.Length.Should().Be(limit);
            }
            else
            {
                dataSets.Length.Should().BeGreaterThan(0);
            }

            var ds = dataSets.First();
            ds.Id.Should().BeGreaterThan(0);
            ds.Name.Should().NotBeEmpty();
            ds.Status.Should().NotBeEmpty();
            ds.ModelGroups.Should().NotBeEmpty();
            ds.NumModelGroups.Should().BeGreaterThan(0);
            ds.RowCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task AddFiles_ShouldAddFiles()
        {
            var dataSet = await _dataHelper.DataSets().GetAny();
            var files = new[] {_dataHelper.Files().GetSampleFilePath()};

            var result = await _dataSetClient.AddFilesAsync(dataSet.Id, files, default);

            result.AddDatasetFiles.Id.Should().Be(dataSet.Id);
        }

        [Test]
        public async Task ProcessFiles_ShouldStartProcessing()
        {
            // Arrange
            var dataSet = await _dataHelper.DataSets().GetAny();
            var files = new[] {_dataHelper.Files().GetSampleFilePath()};
            await _dataSetClient.AddFilesAsync(dataSet.Id, files, default);
            var downloadedFiles =
                (await _dataSetClient.FileUploadStatusAsync(dataSet.Id, default))
                .Dataset.Files
                .Where(f => f.Status == FileStatus.Downloaded)
                .Select(f => f.Id.Value);
            
            // Act
            var result = await _dataSetClient.ProcessFileAsync(dataSet.Id, downloadedFiles, default);

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public async Task ProcessCsv_ShouldStartProcessing()
        {
            // Arrange
            var dataSet = await _dataHelper.DataSets().GetAny();
            var files = new[] { _dataHelper.Files().GetSampleCsvPath() };
            await _dataSetClient.AddFilesAsync(dataSet.Id, files, default);
            var downloadedFiles =
                (await _dataSetClient.FileUploadStatusAsync(dataSet.Id, default))
                .Dataset.Files
                .Where(f => f.Status == FileStatus.Downloaded)
                .Select(f => f.Id.Value);

            // Act
            var result = await _dataSetClient.ProcessCsvAsync(dataSet.Id, downloadedFiles, default);

            // Assert
            result.Should().NotBeNull();
        }
    }
}
