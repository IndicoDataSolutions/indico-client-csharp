using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake.Exceptions;
using IndicoV2.StrawberryShake.IndicoGqlClient;

namespace IndicoV2.StrawberryShake.DataSets.Wrappers
{
    public class AddFilesClient : ErrorHandlingWrapper<IAddFilesMutation>, IAddFilesClient
    {
        public AddFilesClient(AddFilesMutation inner) : base(inner)
        { }

        public Task<IAddFilesResult> ExecuteAsync(int datasetId, string metadata, CancellationToken cancellationToken = default) => 
            ExecuteAsync(() => _inner.ExecuteAsync(datasetId, metadata, cancellationToken));
    }
}
