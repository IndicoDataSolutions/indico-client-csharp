using Indico;
using Indico.Entity;
using Indico.Jobs;
using Indico.Mutation;
using Indico.Query;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Examples
{
    internal class SubmitWorkflows
    {
        private static async Task Main(string[] args)
        {
            var config = new IndicoConfig(
                host: "app.indico.io"
            );

            var client = new IndicoClient(config);

            // List Workflows for Dataset 1707
            var listWorkflows = new ListWorkflows(client)
            {
                DatasetIds = new List<int>() { 1707 }
            };
            var workflows = await listWorkflows.Exec();

            // Select Workflow
            var workflow = workflows[0];

            var workflowSubmission = new WorkflowSubmission(client)
            {
                WorkflowId = workflow.Id,
                // Submit files to Workflow
                Files = new List<string>() { "path-to-file" },
                // Or submit streams to Workflow
                Streams = null // Stream List
            };

            var submissionIds = await workflowSubmission.Exec();

            // Select Submission
            int submissionId = submissionIds[0];

            var submissionResult = new SubmissionResult(client)
            {
                SubmissionId = submissionId
            };

            var job = await submissionResult.Exec();

            // Get Submission result
            var result = await job.Result();
            // Or results
            var results = await job.Results();
        }
    }
}
