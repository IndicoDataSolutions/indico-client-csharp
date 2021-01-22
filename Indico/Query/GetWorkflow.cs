using Indico.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Indico.Query
{
    public class GetWorkflow : IQuery<Workflow>
    {
        private readonly IndicoClient _client;
        public int WorkflowId { get; set; }

        public GetWorkflow(IndicoClient client) => _client = client;

        public async Task<Workflow> Exec()
        {
            var listWorkflows = new ListWorkflows(_client)
            {
                WorkflowIds = new List<int> { WorkflowId }
            };

            var workflows = await listWorkflows.Exec();
            if (workflows.Count != 0)
            {
                return workflows[0];
            }
            return null;
        }
    }
}
