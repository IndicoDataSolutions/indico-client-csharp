using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IndicoV2;
using IndicoV2.Submissions.Models;

namespace Examples
{
    public class SubmitWorkflows
    {
        private static string GetToken() =>
            File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "indico_api_token.txt"));

        public static async Task Main()
        {
            var client = new IndicoClient(GetToken(), new Uri("https://try.indico.io"));

            var dataSets = await client.DataSets().ListAsync();

            var workflows = await client.Workflows().ListAsync(dataSets.First().Id);

            var submissionClient = client.Submissions();

            var submissionIds = await submissionClient.CreateAsync(workflows.Single().Id, new[] { "workflow-sample.pdf" });
            int submissionId = submissionIds.Single();
            var submission = await submissionClient.GetAsync(submissionId);
            var jobResult = await client.GetSubmissionResultAwaiter().WaitReady(submissionId);
            Console.WriteLine(jobResult);
        }
    }
}
