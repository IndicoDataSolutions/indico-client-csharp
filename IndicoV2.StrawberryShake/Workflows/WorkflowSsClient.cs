using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using IndicoV2.StrawberryShake.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using Newtonsoft.Json.Linq;

namespace IndicoV2.StrawberryShake.Workflows
{
    public class WorkflowSsClient : ErrorHandlingWrapper
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

        public async Task<IListWorkflows_Workflows> GetWorkflowAsync(int workflowId, CancellationToken cancellationToken)
        {
            int?[] workflowIds = { workflowId };
            var response = await ExecuteAsync(() =>
            _services.GetRequiredService<ListWorkflowsQuery>().ExecuteAsync(null, Array.AsReadOnly(workflowIds), cancellationToken));
            return response.Workflows;
        }

        public async Task<IListWorkflows_Workflows> ListAsync(int datasetId, CancellationToken cancellationToken)
        {
            int?[] datasetIds =  { datasetId };
            var response = await ExecuteAsync(() =>
                _services.GetRequiredService<ListWorkflowsQuery>().ExecuteAsync(Array.AsReadOnly(datasetIds), null, cancellationToken));
            return response.Workflows;
        }

        public async Task<IListWorkflows_Workflows> ListAsync(int[] datasetIds, CancellationToken cancellationToken)
        {
            var nullableDatasetIds = datasetIds.Cast<int?>().ToArray();
            var response = await ExecuteAsync(() =>
                _services.GetRequiredService<ListWorkflowsQuery>().ExecuteAsync(Array.AsReadOnly(nullableDatasetIds), null, cancellationToken));
            return response.Workflows;
        }

    }
}
