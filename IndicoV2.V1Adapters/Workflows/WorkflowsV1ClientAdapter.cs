using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Indico;
using Indico.Query;
using IndicoV2.V1Adapters.Workflows.Models;
using IndicoV2.Workflows.Models;

namespace IndicoV2.V1Adapters.Workflows
{
    public class WorkflowsV1ClientAdapter
    {
        private readonly IndicoClient _indicoClientLegacy;

        public WorkflowsV1ClientAdapter(IndicoClient indicoClientLegacy) => _indicoClientLegacy = indicoClientLegacy;

        public Task<IEnumerable<IWorkflow>> ListAsync(int dataSetId, CancellationToken cancellationToken = default) =>
            ListAsync(new[] {dataSetId}, cancellationToken);

        public async Task<IEnumerable<IWorkflow>> ListAsync(int[] dataSetIds, CancellationToken cancellationToken = default)
        {
            var workflows = await new ListWorkflows(_indicoClientLegacy)
            {
                DatasetIds = dataSetIds?.ToList()
            }.Exec(cancellationToken);

            return workflows.Select(wf => new V1WorkflowAdapter(wf));
        }
    }
}