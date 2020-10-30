using Indico.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Indico.Query
{
    public class GetWorkflow : Query<Workflow>
    {
        IndicoClient _client;
        public int WorkflowId { get; set; }

        public GetWorkflow(IndicoClient client) => this._client = client;

        public async Task<Workflow> Exec()
        {
            ListWorkflows listWorkflows = new ListWorkflows(this._client)
            {
                WorkflowIds = new List<int> { this.WorkflowId }
            };

            List<Workflow> workflows = await listWorkflows.Exec();
            if (workflows.Count != 0)
            {
                return workflows[0];
            }
            return null;
        }

        public Task<Workflow> Refresh(Workflow obj)
        {
            throw new NotImplementedException();
        }
    }
}
