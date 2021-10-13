using System;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake;
using IndicoV2.Workflows;

namespace IndicoV2.Extensions.Workflows
{
    public class WorkflowAwaiter
    {
        private readonly IWorkflowsClient _workflows;

        public WorkflowAwaiter(IWorkflowsClient workflows) => _workflows = workflows;

        public async Task WaitWorkflowCompleteAsync(int workflowId, TimeSpan checkInterval, CancellationToken cancellationToken)
        {
            while ((await _workflows.GetStatusAsync(workflowId, cancellationToken)) != WorkflowStatus.Complete)
            {
                await Task.Delay(checkInterval, cancellationToken);
            }
        }
    }
}
