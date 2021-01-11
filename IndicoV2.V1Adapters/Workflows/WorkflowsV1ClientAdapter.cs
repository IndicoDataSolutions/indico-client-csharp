using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Indico;
using Indico.Query;
using IndicoV2.Abstractions.Workflows;
using IndicoV2.Abstractions.Workflows.Models;
using IndicoV2.V1Adapters.Workflows.Models;

namespace IndicoV2.V1Adapters.Workflows
{
    public class WorkflowsV1ClientAdapter : IWorkflowsClient
    {
        private readonly IndicoClient _indicoClientLegacy;

        public WorkflowsV1ClientAdapter(Indico.IndicoClient indicoClientLegacy)
        {
            _indicoClientLegacy = indicoClientLegacy;
        }

        public Task<IEnumerable<Workflow>> ListAsync(int dataSetId, CancellationToken cancellationToken = default) =>
            ListAsync(new[] {dataSetId}, cancellationToken);

        public async Task<IEnumerable<Workflow>> ListAsync(int[] datasetIds, CancellationToken cancellationToken = default)
        {
            var workflows = await new ListWorkflows(_indicoClientLegacy)
            {
                DatasetIds = datasetIds.ToList()
            }.Exec();

            return workflows.Select(wf => new V1WorkflowAdapter(wf));
        }
    }
}