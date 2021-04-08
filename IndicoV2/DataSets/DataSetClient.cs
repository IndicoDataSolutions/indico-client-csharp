using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.DataSets.Models;
using IndicoV2.Storage;
using IndicoV2.StrawberryShake;
using IndicoV2.StrawberryShake.DataSets;
using IndicoV2.V1Adapters.DataSets;

namespace IndicoV2.DataSets
{
    public class DataSetClient : IDataSetClient
    {
        private readonly DataSetsV1ClientAdapter _legacyAdapter;
        private readonly IDataSetSsClient _dataSetSsClient;
        private readonly IStorageClient _storage;

        public DataSetClient(DataSetsV1ClientAdapter legacyAdapter, IDataSetSsClient dataSetSsClient, IStorageClient storage)
        {
            _legacyAdapter = legacyAdapter;
            _dataSetSsClient = dataSetSsClient;
            _storage = storage;
        }

        public Task<IEnumerable<IDataSet>> ListAsync(CancellationToken cancellationToken = default) =>
            _legacyAdapter.ListAsync(cancellationToken);

        public Task<IEnumerable<IDataSetFull>> ListFullAsync(int? limit = null, CancellationToken cancellationToken = default) =>
            _legacyAdapter.ListFullAsync(limit, cancellationToken);

        public async Task<IAddFilesResult> AddFilesAsync(int dataSetId, IEnumerable<string> filePaths,
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

        public Task<IProcessFilesResult> ProcessFileAsync(int dataSetId, IEnumerable<int> fileIds, CancellationToken cancellationToken) => 
            _dataSetSsClient.ProcessFilesAsync(dataSetId, fileIds, cancellationToken);

        public Task<IProcessCsvResult> ProcessCsvAsync(int dataSetId, IEnumerable<int> fileIds, CancellationToken cancellationToken) =>
            _dataSetSsClient.ProcessCsvAsync(dataSetId, fileIds, cancellationToken);
    }
}
