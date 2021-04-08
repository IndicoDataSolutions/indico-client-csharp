using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.DataSets;
using IndicoV2.StrawberryShake.IndicoGqlClient;

namespace IndicoV2.Extensions.DataSets
{
    public class DataSetAwaiter : IDataSetAwaiter
    {
        private readonly List<FileStatus> _statusesFinished = new List<FileStatus>{ FileStatus.Processed, FileStatus.Failed, FileStatus.Downloaded };
        private readonly IDataSetClient _dataSets;

        public DataSetAwaiter(IDataSetClient dataSets) => _dataSets = dataSets;

        public async Task WaitAllFilesProcessedAsync(int dataSetId, TimeSpan checkInterval, CancellationToken cancellationToken)
        {
            while(!CheckAllFilesFinished(await _dataSets.FileUploadStatus(dataSetId, cancellationToken)))
            {
                await Task.Delay(checkInterval, cancellationToken);
            }
        }

        private bool CheckAllFilesFinished(IDatasetUploadStatusResult dataSetUploadStatus) =>
            dataSetUploadStatus.Dataset.Files.All(f => _statusesFinished.Contains(f.Status.Value));
    }
}
