using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Workflows.Models;

namespace IndicoV2.Workflows
{
    public interface IWorkflowsClient
    {
        Task<IEnumerable<IWorkflow>> ListAsync(int dataSetId, CancellationToken cancellationToken = default);

        Task<IEnumerable<IWorkflow>> ListAsync(int[] dataSetIds, CancellationToken cancellationToken = default);
    }
}