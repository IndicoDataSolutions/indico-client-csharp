using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake.Exceptions;
using IndicoV2.StrawberryShake.IndicoGqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace IndicoV2.StrawberryShake.DataSets
{
    public class DataSetClientGql : ErrorHandlingWrapper, IDataSetClientGql
    {
        private readonly ServiceProvider _services;

        public DataSetClientGql(ServiceProvider services) => _services = services;

        public async Task<IAddFilesResult> AddFiles(int dataSetId, string metaData, CancellationToken cancellationToken) =>
            await ExecuteAsync(async () =>
                await _services.GetRequiredService<AddFilesMutation>()
                    .ExecuteAsync(dataSetId, metaData, cancellationToken));

        public async Task<IDatasetUploadStatusResult> FileUploadStatus(int id, CancellationToken cancellationToken) =>
            await ExecuteAsync(async () =>
                await _services.GetRequiredService<DatasetUploadStatusQuery>().ExecuteAsync(id, cancellationToken));
    }
}
