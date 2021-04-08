using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace IndicoV2.StrawberryShake.DataSets
{
    public class DataSetSsClient : ErrorHandlingWrapper, IDataSetSsClient
    {
        private readonly ServiceProvider _services;

        public DataSetSsClient(ServiceProvider services) => _services = services;

        public async Task<IAddFilesResult> AddFiles(int dataSetId, string metaData, CancellationToken cancellationToken) =>
            await ExecuteAsync(async () =>
                await _services.GetRequiredService<AddFilesMutation>()
                    .ExecuteAsync(dataSetId, metaData, cancellationToken));

        public async Task<IDatasetUploadStatusResult> FileUploadStatus(int id, CancellationToken cancellationToken) =>
            await ExecuteAsync(async () =>
                await _services.GetRequiredService<DatasetUploadStatusQuery>().ExecuteAsync(id, cancellationToken));

        public Task<IProcessFilesResult> ProcessFilesAsync(int dataSetId, IEnumerable<int> fileIds, CancellationToken cancellationToken) =>
            ExecuteAsync(() => _services.GetRequiredService<ProcessFilesMutation>().ExecuteAsync(
                dataSetId, 
                fileIds.Cast<int?>().ToList(), 
                cancellationToken));

        public Task<IProcessCsvResult> ProcessCsvAsync(int dataSetId, IEnumerable<int> fileIds, CancellationToken cancellationToken) =>
            ExecuteAsync(() => _services.GetRequiredService<ProcessCsvMutation>().ExecuteAsync(
                dataSetId,
                fileIds.Cast<int?>().ToList(),
                cancellationToken));
    }
}
