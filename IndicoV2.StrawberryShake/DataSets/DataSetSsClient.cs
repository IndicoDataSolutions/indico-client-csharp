using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace IndicoV2.StrawberryShake.DataSets
{
    public class DataSetSsClient : ErrorHandlingWrapper
    {
        private readonly ServiceProvider _services;

        public DataSetSsClient(ServiceProvider services) => _services = services;

        public async Task<IDataSetAddFilesResult> AddFiles(int dataSetId, string metaData, CancellationToken cancellationToken) =>
            await ExecuteAsync(async () =>
                await _services.GetRequiredService<DataSetAddFilesMutation>()
                    .ExecuteAsync(dataSetId, metaData, cancellationToken));

        public async Task<IDatasetUploadStatusResult> FileUploadStatus(int id, CancellationToken cancellationToken) =>
            await ExecuteAsync(async () =>
                await _services.GetRequiredService<DatasetUploadStatusQuery>().ExecuteAsync(id, cancellationToken));

        public Task<IDataSetProcessFilesResult> ProcessFilesAsync(int dataSetId, IEnumerable<int> fileIds, CancellationToken cancellationToken) =>
            ExecuteAsync(() => _services.GetRequiredService<DataSetProcessFilesMutation>().ExecuteAsync(
                dataSetId, 
                fileIds.Cast<int?>().ToList(), 
                cancellationToken));

        public Task<IDataSetProcessCsvResult> ProcessCsvAsync(int dataSetId, IEnumerable<int> fileIds, CancellationToken cancellationToken) =>
            ExecuteAsync(() => _services.GetRequiredService<DataSetProcessCsvMutation>().ExecuteAsync(
                dataSetId,
                fileIds.Cast<int?>().ToList(),
                cancellationToken));
    }
}
