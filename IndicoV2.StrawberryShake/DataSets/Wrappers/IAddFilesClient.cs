using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake.IndicoGqlClient;

namespace IndicoV2.StrawberryShake.DataSets.Wrappers
{
    public interface IAddFilesClient
    {
        Task<IAddFilesResult> ExecuteAsync(int dataSetId, string metadata, CancellationToken cancellationToken = default);
    }
}