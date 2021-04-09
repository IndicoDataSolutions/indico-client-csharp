using System.Threading;
using System.Threading.Tasks;

namespace IndicoV2.StrawberryShake.Workflows
{
    public interface IWorkflowSsClient
    {
        Task<IWorkflowAddDataResult> AddData(int workflowId, CancellationToken cancellationToken);
        Task<WorkflowStatus> GetStatus(int workflowId, CancellationToken cancellationToken);
    }
}