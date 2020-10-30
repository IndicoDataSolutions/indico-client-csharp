using Indico;
using Indico.Entity;
using Indico.Jobs;
using Indico.Mutation;
using Indico.Query;
using System;
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

            // List Workflows for Dataset
            ListWorkflows listWorkflows = new ListWorkflows(client)
            {
                DatasetIds = new List<int>() { DATASET_ID }
            };
            List<Workflow> workflows = await listWorkflows.Exec();

            // Select Workflow
            Workflow workflow = workflows[0];

            // Submit files to Workflow
            WorkflowSubmission workflowSubmission = new WorkflowSubmission(client)
            {
                WorkflowId = workflow.Id,
                Files = FILE_LIST
            };

            List<int> submissionIds = await workflowSubmission.Exec();
        }
    }
}
