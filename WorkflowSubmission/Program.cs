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
    class SubmitWorkflows
    {
        async static Task Main(string[] args)
        {
            IndicoConfig config = new IndicoConfig(
                host: "app.indico.io"
            );

            IndicoClient client = new IndicoClient(config);

            // List Workflows for Dataset 1707
            ListWorkflows listWorkflows = new ListWorkflows(client)
            {
                DatasetIds = new List<int>() { 1707 }
            };
            List<Workflow> workflows = await listWorkflows.Exec();

            // Select Workflow
            Workflow workflow = workflows[0];

            WorkflowSubmission workflowSubmission = new WorkflowSubmission(client)
            {
                WorkflowId = workflow.Id,
                // Submit files to Workflow
                Files = new List<string>() { "path-to-file" },
                // Or submit streams to Workflow
                Streams = // Stream List
            };

            List<int> submissionIds = await workflowSubmission.Exec();

            // Select Submission
            int submissionId = submissionIds[0];

            SubmissionResult submissionResult = new SubmissionResult(client)
            {
                SubmissionId = submissionId
            };

            Job job = await submissionResult.Exec();

            // Get Submission result
            JObject result = await job.Result();
            // Or results
            JArray results = await job.Results();
        }
    }
}
