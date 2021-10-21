using Indico.Entity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Indico.Query
{
    /// <summary>
    /// Gets workflow.
    /// </summary>
    /// 
    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    public class GetWorkflow : IQuery<Workflow>
    {
        private readonly IndicoClient _client;
        private int? _workflowId;

        /// <summary>
        /// Workflow id.
        /// </summary>
        public int WorkflowId 
        {
            get
            {
                if (!_workflowId.HasValue)
                {
                    throw new ArgumentNullException(nameof(WorkflowId));
                }

                return _workflowId.Value;
            }

            set => _workflowId = value;
        }

        /// <summary>
        /// GetWorkflow constructor.
        /// </summary>
        /// <param name="client">Client used to send API requests.</param>
        public GetWorkflow(IndicoClient client) => _client = client;

        /// <summary>
        /// Executes query and returns workflow.
        /// </summary>
        public async Task<Workflow> Exec(CancellationToken cancellationToken = default)
        {
            var listWorkflows = new ListWorkflows(_client)
            {
                WorkflowIds = new List<int> { WorkflowId }
            };

            var workflows = await listWorkflows.Exec(cancellationToken);
            if (workflows.Count != 0)
            {
                return workflows[0];
            }

            return null;
        }
    }
}
