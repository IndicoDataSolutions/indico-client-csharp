﻿using System;
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
            var client = new IndicoClient(null, new Uri("https://app.indico.io"));

            var dataSets = await client.DataSets().ListAsync();

            var workflows = await client.Workflows().ListAsync(dataSets.First().Id);

            var submissionClient = client.Submissions();

            var submissionIds = await submissionClient.CreateAsync(workflows.Single().Id, new[] {"workflow-sample.pdf"});
            int submissionId = submissionIds.Single();

            var submission = await submissionClient.GetAsync(submissionId);
            var job = await submissionClient.GetJobWhenReady(submissionId, timeout: TimeSpan.FromSeconds(5));
            Console.ReadLine();
        }
    }
}
