using System;
using System.Linq;
using System.Threading.Tasks;
using IndicoV2;
using IndicoV2.Submissions;

namespace Examples
{
    internal class SubmitWorkflows
    {
        public static async Task Main()
        {
            // null token will be replaced with an actual token by V1 config mechanism
            var client = new IndicoClient(new Uri("https://app.indico.io"), null);

            var dataSets = await client.DataSets().ListAsync();

            var workflows = await client.Workflows().ListAsync(dataSets.First().Id);

            var submissionClient = client.Submissions();

            var submissionIds = await submissionClient.CreateAsync(workflows.Single().Id, new[] {"SubmitWorkflows.runtimeconfig.json"});
            int submissionId = submissionIds.Single();

            var submission = await submissionClient.GetAsync(submissionId);
            var job = await submissionClient.GetJobWhenReady(submissionId);
            Console.ReadLine();
        }
    }
}
