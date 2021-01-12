using Indico.Entity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Indico.Query
{
    public class GetWorkflow : Query<Workflow>
    {
        IndicoClient _client;
        public int WorkflowId { get; set; }

        public GetWorkflow(IndicoClient client) => this._client = client;

        public async Task<Workflow> Exec(CancellationToken cancellationToken = default)
        {
            ListWorkflows listWorkflows = new ListWorkflows(this._client)
            {
                WorkflowIds = new List<int> { this.WorkflowId }
            };

            List<Workflow> workflows = await listWorkflows.Exec(cancellationToken);
            if (workflows.Count != 0)
            {
                return workflows[0];
            }
            return null;
        }
    }
}
