using System;
using System.Linq;
using System.Threading.Tasks;
using IndicoV2;
using IndicoV2.Submissions;

namespace Examples
{
    class SubmitWorkflows
    {
        static async Task Main()
        {
            var client = new IndicoClient(new Uri("https://app.indico.io"));

            var dataSets = await client.DataSets().ListAsync();

            var workflows = await client.Workflows().ListAsync(dataSets.First().Id);

            var submissionClient = client.Submissions();

            var submissionIds = await submissionClient.CreateAsync(workflows.Single().Id, new[] {"SubmitWorkflows.runtimeconfig.json"});
            var submissionId = submissionIds.Single();

            var submission = await submissionClient.GetAsync(submissionId);
            var job = await submissionClient.GetJobWhenReady(submissionId);
            Console.ReadLine();
        }
    }
}
