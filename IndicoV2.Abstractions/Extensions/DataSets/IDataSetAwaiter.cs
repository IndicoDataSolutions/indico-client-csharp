using System;
using System.Threading;
using System.Threading.Tasks;

namespace IndicoV2.Extensions.DataSets
{
    public interface IDataSetAwaiter
    {
        Task WaitAllFilesProcessedAsync(int dataSetId, TimeSpan checkInterval, CancellationToken cancellationToken);
    }
}