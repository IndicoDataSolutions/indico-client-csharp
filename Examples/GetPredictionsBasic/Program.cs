using System;
using System.IO;
using System.Threading.Tasks;
using IndicoV2;

namespace Examples
{
    /// <summary>
    /// Returns raw output.
    /// </summary>
    public class GetPredictionsBasic
    {
        private static string GetToken() =>
            File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "indico_api_token.txt"));

        public static async Task Main()
        {
            var client = new IndicoClient(GetToken(), new Uri("https://try.indico.io"));
            int submissionId = 91345;
            var jobResult = await client.GetSubmissionResultAwaiter().WaitReady(submissionId);
            Console.WriteLine(jobResult);
        }
    }
}
