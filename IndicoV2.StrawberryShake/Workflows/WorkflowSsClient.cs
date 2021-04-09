using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace IndicoV2.StrawberryShake.Workflows
{
    public class WorkflowSsClient : ErrorHandlingWrapper, IWorkflowSsClient
    {
        private readonly ServiceProvider _services;

        public WorkflowSsClient(ServiceProvider services) => _services = services;

        public Task<IWorkflowAddDataResult> AddData(int workflowId, CancellationToken cancellationToken) => 
            ExecuteAsync(async () => await _services
            .GetRequiredService<WorkflowAddDataMutation>().ExecuteAsync(workflowId, cancellationToken));

        public async Task<WorkflowStatus> GetStatus(int workflowId, CancellationToken cancellationToken)
        {
            var response = await ExecuteAsync(() =>
                _services.GetRequiredService<WorkflowGetStatusQuery>().ExecuteAsync(workflowId, cancellationToken));

            return response.Workflows.Workflows.Single().Status.Value;
        }
    }
}
