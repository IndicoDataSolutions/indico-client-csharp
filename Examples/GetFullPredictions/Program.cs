using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IndicoV2;
using Newtonsoft.Json.Linq;

namespace Examples
{
    /// <summary>
    /// Returns full output with pre_review, post_review, and final predictions
    /// </summary>
    public class GetFullPredictions
    {
        private static string GetToken() =>
            File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "indico_api_token.txt"));

        public static async Task Main()
        {
            var client = new IndicoClient(GetToken(), new Uri("https://app.indico.io"));

            var dataSets = await client.DataSets().ListAsync();

            var workflows = await client.Workflows().ListAsync(dataSets.First().Id);

            var submissionClient = client.Submissions();

            var jobClient = client.Jobs();

            var storageClient = client.Storage();

            var submissionIds = await submissionClient.CreateAsync(workflows.Single().Id, new[] { "workflow-sample.pdf" });
            int submissionId = submissionIds.Single();
            var submission = await submissionClient.GetAsync(submissionId);

            string jobId = await submissionClient.GenerateSubmissionResultAsync(submissionId);
            var jobResult = await jobClient.GetResultAsync<dynamic>(jobId);
            var jobResultUrl = jobResult.Value("url");

            var storageResult = await storageClient.GetAsync(new Uri(jobResultUrl), default);
            using (var reader = new StreamReader(storageResult))
            {
                string resultAsString = reader.ReadToEnd();
                JObject resultObject = JObject.Parse(resultAsString);
                Console.WriteLine(resultObject);
            }
        }
    }
}
