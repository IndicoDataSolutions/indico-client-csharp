using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Abstractions.Workflows.Models;

namespace IndicoV2.Abstractions.Workflows
{
    public interface IWorkflowsClient
    {
        Task<IEnumerable<Workflow>> ListAsync(int dataSetId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Workflow>> ListAsync(int[] dataSetIds, CancellationToken cancellationToken = default);
    }
}