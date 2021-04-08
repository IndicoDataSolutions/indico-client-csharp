using System;
using System.Threading;
using System.Threading.Tasks;

namespace IndicoV2.Extensions.DataSets
{
    public interface IDataSetAwaiter
    {
        Task WaitFilesDownloadedOrFailedAsync(int dataSetId, TimeSpan checkInterval, CancellationToken cancellationToken);
        Task WaitFilesProcessedOrFailedAsync(int datasSetId, TimeSpan checkInterval, CancellationToken cancellationToken);
    }
}