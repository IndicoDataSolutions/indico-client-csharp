using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.DataSets.Models;
using IndicoV2.Models.Models;
using IndicoV2.Storage;
using IndicoV2.StrawberryShake;
using IndicoV2.StrawberryShake.DataSets;

namespace IndicoV2.DataSets
{
    public class DataSetClient : IDataSetClient
    {
        private readonly DataSetSsClient _dataSetSsClient;
        private readonly IStorageClient _storage;

        public DataSetClient(DataSetSsClient dataSetSsClient, IStorageClient storage)
        {
            _dataSetSsClient = dataSetSsClient;
            _storage = storage;
        }

        public async Task<IEnumerable<IDataSet>> ListAsync(CancellationToken cancellationToken = default)
        {
            var datasets = await _dataSetSsClient.ListAsync(cancellationToken);
            var result = datasets?.Datasets.Select(x => ToDataSetFromGetDatasetResult(x)).ToList() ?? new List<IDataSet>();
            return result;
        }

        public async Task<IEnumerable<IDataSetFull>> ListFullAsync(int? limit = null, CancellationToken cancellationToken = default)
        {
            var datasets = await _dataSetSsClient.ListFullAsync(limit, cancellationToken);
            var result = datasets?.DatasetsPage.Datasets.Select(x => ToDataSetFullFromGetDatasetFullResult(x)).ToList() ?? new List<IDataSetFull>();
            return result;
        }

        public async Task<IDataSetAddFilesResult> AddFilesAsync(int dataSetId, IEnumerable<string> filePaths,
            CancellationToken cancellationToken)
        {
            var uploadedFiles = await _storage.UploadAsync(filePaths, cancellationToken);
            var metadata = _storage.Serialize(uploadedFiles);
            var result = await _dataSetSsClient.AddFiles(dataSetId, metadata.ToString(), cancellationToken);

            return result;
        }

        public Task<IDatasetUploadStatusResult> FileUploadStatusAsync(int dataSetId,
            CancellationToken cancellationToken) =>
            _dataSetSsClient.FileUploadStatus(dataSetId, cancellationToken);

        public Task<IDataSetProcessFilesResult> ProcessFileAsync(int dataSetId, IEnumerable<int> fileIds, CancellationToken cancellationToken) =>
            _dataSetSsClient.ProcessFilesAsync(dataSetId, fileIds, cancellationToken);

        public Task<IDataSetProcessCsvResult> ProcessCsvAsync(int dataSetId, IEnumerable<int> fileIds, CancellationToken cancellationToken) =>
            _dataSetSsClient.ProcessCsvAsync(dataSetId, fileIds, cancellationToken);

        private IDataSet ToDataSetFromGetDatasetResult(IDataSetGetDatasets_Datasets dataset) => new DataSet
        {
            Id = dataset.Id ?? 0,
            Name = dataset.Name
        };

        private IDataSetFull ToDataSetFullFromGetDatasetFullResult(IDataSetGetDatasetsFull_DatasetsPage_Datasets dataset) => new DataSetFull
        {
            Id = dataset.Id ?? 0,
            Name = dataset.Name,
            NumModelGroups = dataset.NumModelGroups ?? 0,
            RowCount = dataset.RowCount ?? 0,
            Status = dataset.Status.ToString(),
            ModelGroups = dataset.ModelGroups.Select(x => new ModelGroupBase{ Id = x.Id ?? 0}).ToArray() ?? new ModelGroupBase[0],
        };


    }
}
