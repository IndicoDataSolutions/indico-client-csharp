using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IndicoV2;
using Newtonsoft.Json.Linq;

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

            var submissionClient = client.Submissions();

            var storageClient = client.Storage();

            int submissionId = 152070;
            var submission = await submissionClient.GetAsync(submissionId);

            string resultFileUrl = submission.ResultFile;
            var storageResult = await storageClient.GetAsync(new Uri(resultFileUrl), default);
            using (var reader = new StreamReader(storageResult))
            {
                string resultAsString = reader.ReadToEnd();
                JObject resultObject = JObject.Parse(resultAsString);
                Console.WriteLine(resultObject);
            }
        }
    }
}
