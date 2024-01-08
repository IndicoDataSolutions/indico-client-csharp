using System;
using System.IO;
using System.Threading.Tasks;
using IndicoV2;
using Newtonsoft.Json.Linq;

namespace Examples
{
    /// <summary>
    /// Returns full output with pre_review, post_review, and final predictions
    /// </summary>
    public class GetPredictionsWithReview
    {
        private static string GetToken() =>
            File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "indico_api_token.txt"));

        public static async Task Main()
        {
            var client = new IndicoClient(GetToken(), new Uri("https://try.indico.io"));
            int submissionId = 91345;

            string jobId = await client.Submissions().GenerateSubmissionResultAsync(submissionId);
            var jobResult = await client.Jobs().GetResultAsync(jobId, default);
            string jobResultUrl = JToken.Parse(jobResult).Value<string>("url");

            var storageResult = await client.Storage().GetAsync(new Uri(jobResultUrl), default);
            using (var reader = new StreamReader(storageResult))
            {
                string resultAsString = reader.ReadToEnd();
                var resultObject = JObject.Parse(resultAsString);
                Console.WriteLine(resultObject);
            }
        }
    }
}
