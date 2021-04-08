using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.DataSets.Models;
using IndicoV2.Storage;
using IndicoV2.StrawberryShake.IndicoGqlClient;
using IndicoV2.V1Adapters.DataSets;
using Newtonsoft.Json.Linq;
using Gql = IndicoV2.StrawberryShake.DataSets.Wrappers;

namespace IndicoV2.DataSets
{
    public class DataSetClient : IDataSetClient
    {
        private readonly DataSetsV1ClientAdapter _legacyAdapter;
        private readonly Gql.IAddFilesClient _addFilesClient;
        private readonly IStorageClient _storage;

        public DataSetClient(DataSetsV1ClientAdapter legacyAdapter, Gql.IAddFilesClient addFilesClient, IStorageClient storage)
        {
            _legacyAdapter = legacyAdapter;
            _addFilesClient = addFilesClient;
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
            var result = await _addFilesClient.ExecuteAsync(dataSetId, metadata.ToString(), cancellationToken);

            return result;
        }
    }
}
