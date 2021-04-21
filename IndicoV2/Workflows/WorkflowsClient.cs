using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake;
using IndicoV2.V1Adapters.Workflows;
using IndicoV2.Workflows.Models;

namespace IndicoV2.Workflows
{
    public class WorkflowsClient : IWorkflowsClient
    {
        private readonly WorkflowsV1ClientAdapter _legacy;
        private readonly IndicoStrawberryShakeClient _strawberryShake;

        public WorkflowsClient(IndicoClient indicoClient)
        {
            _legacy = new WorkflowsV1ClientAdapter(indicoClient.LegacyClient);
            _strawberryShake = indicoClient.IndicoStrawberryShakeClient;
        }

        public Task<IEnumerable<IWorkflow>> ListAsync(int dataSetId, CancellationToken cancellationToken = default) =>
            _legacy.ListAsync(dataSetId, cancellationToken);

        public Task<IEnumerable<IWorkflow>>
            ListAsync(int[] dataSetIds, CancellationToken cancellationToken = default) =>
            _legacy.ListAsync(dataSetIds, cancellationToken);

        public Task<IWorkflowAddDataResult> AddDataAsync(int workflowId, CancellationToken cancellationToken) =>
            _strawberryShake.Workflows().AddData(workflowId, cancellationToken);

        public Task<WorkflowStatus> GetStatusAsync(int workflowId, CancellationToken cancellationToken) =>
            _strawberryShake.Workflows().GetStatus(workflowId, cancellationToken);
    }
}
