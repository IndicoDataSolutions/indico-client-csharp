using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IndicoV2;
using IndicoV2.StrawberryShake;

namespace Examples
{
    public class Program
    {
        private static async Task Main()
        {
            Console.WriteLine("Adding files to the DataSet.");
            var token = File.ReadAllText(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "indico_api_token.txt"));
            var client = new IndicoClient(token);
            var dataSetsClient = client.DataSets();
            var dataSetAwaiter = client.DataSetAwaiter();

            var dataSets = await dataSetsClient.ListFullAsync(1);
            var dataSetId = dataSets.Single().Id;

            await dataSetsClient.AddFilesAsync(dataSetId, new[] { "workflow-sample.pdf" }, default);
            await dataSetAwaiter.WaitFilesDownloadedOrFailedAsync(dataSetId, TimeSpan.FromSeconds(0.5), default);

            var statusesResult = await dataSetsClient.FileUploadStatusAsync(dataSetId, default);
            var downloadedFileIds = statusesResult.Dataset.Files
                .Where(f => f.Status == FileStatus.Downloaded)
                .Select(f => f.Id.Value);

            await dataSetsClient.ProcessFileAsync(dataSetId, downloadedFileIds, default);
            await dataSetAwaiter.WaitFilesProcessedOrFailedAsync(dataSetId, TimeSpan.FromSeconds(0.5), default);
            Console.WriteLine("Adding files to the DataSet - finished.");
        }
    }
}
