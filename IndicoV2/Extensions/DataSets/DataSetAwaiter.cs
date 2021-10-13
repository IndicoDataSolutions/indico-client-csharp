using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.DataSets;
using IndicoV2.StrawberryShake;

namespace IndicoV2.Extensions.DataSets
{
    public class DataSetAwaiter : IDataSetAwaiter
    {
        private readonly List<FileStatus> _downloadedOrFailed = new List<FileStatus>{ FileStatus.Failed, FileStatus.Downloaded };
        private readonly List<FileStatus> _processedOrFailed = new List<FileStatus> { FileStatus.Failed, FileStatus.Processed };
        private readonly IDataSetClient _dataSets;

        public DataSetAwaiter(IDataSetClient dataSets) => _dataSets = dataSets;

        public Task WaitFilesDownloadedOrFailedAsync(int dataSetId, TimeSpan checkInterval, CancellationToken cancellationToken)
            => WaitAllFilesInStatus(dataSetId, checkInterval, _downloadedOrFailed, cancellationToken);

        public Task WaitFilesProcessedOrFailedAsync(int datasSetId, TimeSpan checkInterval, CancellationToken cancellationToken)
            => WaitAllFilesInStatus(datasSetId, checkInterval, _processedOrFailed, cancellationToken);

        private async Task WaitAllFilesInStatus(int dataSetId, TimeSpan checkInterval, List<FileStatus> statusesAwaited, CancellationToken cancellationToken)
        {
            while (!CheckAllFilesHaveStatus(await _dataSets.FileUploadStatusAsync(dataSetId, cancellationToken), statusesAwaited))
            {
                await Task.Delay(checkInterval, cancellationToken);
            }
        }

        private bool CheckAllFilesHaveStatus(IDatasetUploadStatusResult datasetUploadStatusResult, List<FileStatus> statusesAwaited) => datasetUploadStatusResult.Dataset.Files.All(f => statusesAwaited.Contains(f.Status.Value));
    }
}
