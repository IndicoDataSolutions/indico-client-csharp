using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using IndicoV2.StrawberryShake;
using IndicoV2.Workflows.Models;

namespace IndicoV2.Workflows
{
    public class WorkflowsClient : IWorkflowsClient
    {
        private readonly IndicoStrawberryShakeClient _strawberryShake;
        private readonly IndicoClient _indicoClient;

        public WorkflowsClient(IndicoClient indicoClient)
        {
            _indicoClient = indicoClient;
            _strawberryShake = indicoClient.IndicoStrawberryShakeClient;
        }

        public async Task<IEnumerable<IWorkflow>> ListAsync(int dataSetId, CancellationToken cancellationToken = default) 
        {
            var result = await _strawberryShake.Workflows().ListAsync(dataSetId, cancellationToken);
            return result.Workflows.Select(x => ToWorkflow(x)).ToList();
        }

        public async Task<IEnumerable<IWorkflow>>ListAsync(int[] dataSetIds, CancellationToken cancellationToken = default)
        {
            var result = await _strawberryShake.Workflows().ListAsync(dataSetIds, cancellationToken);
            return result.Workflows.Select(x => ToWorkflow(x)).ToList();
        }

        public Task<IWorkflowAddDataResult> AddDataAsync(int workflowId, CancellationToken cancellationToken) =>
            _strawberryShake.Workflows().AddData(workflowId, cancellationToken);

        public Task<WorkflowStatus> GetStatusAsync(int workflowId, CancellationToken cancellationToken) =>
            _strawberryShake.Workflows().GetStatus(workflowId, cancellationToken);

        private Workflow ToWorkflow(IListWorkflows_Workflows_Workflows workflow) => new Workflow {
            Id = workflow.Id ?? 0,
            ReviewEnabled = workflow.ReviewEnabled ?? false,
            Name = workflow.Name,
        };
    }
}
