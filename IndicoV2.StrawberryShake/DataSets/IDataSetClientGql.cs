using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake.IndicoGqlClient;

namespace IndicoV2.StrawberryShake.DataSets
{
    public interface IDataSetClientGql
    {
        Task<IAddFilesResult> AddFiles(int dataSetId, string metaData, CancellationToken cancellationToken);
        Task<IDatasetUploadStatusResult> FileUploadStatus(int id, CancellationToken cancellationToken);
    }
}