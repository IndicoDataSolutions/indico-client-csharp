using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2;

namespace Examples.AddDataSetFiles
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
            
            var dataSets = await dataSetsClient.ListFullAsync(1);
            var dataSetId = dataSets.Single().Id;
            await dataSetsClient.AddFilesAsync(dataSetId, new[] {"workflow-sample.pdf"}, CancellationToken.None);

            await client.DataSetAwaiter().WaitAllFilesProcessedAsync(dataSetId, TimeSpan.FromSeconds(0.5), default);
            Console.WriteLine("Adding files to the DataSet - finished.");
        }
    }
}
