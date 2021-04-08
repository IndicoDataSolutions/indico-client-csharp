using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IndicoV2.StrawberryShake.DataSets
{
    public interface IDataSetSsClient
    {
        Task<IAddFilesResult> AddFiles(int dataSetId, string metaData, CancellationToken cancellationToken);
        Task<IDatasetUploadStatusResult> FileUploadStatus(int id, CancellationToken cancellationToken);
        Task<IProcessFilesResult> ProcessFilesAsync(int dataSetId, IEnumerable<int> fileIds, CancellationToken cancellationToken);
        Task<IProcessCsvResult> ProcessCsvAsync(int dataSetId, IEnumerable<int> fileIds, CancellationToken cancellationToken);
    }
}