using System.Linq;
using System.Threading.Tasks;
using IndicoV2.DataSets;
using IndicoV2.Workflows;
using IndicoV2.Workflows.Models;

namespace IndicoV2.IntegrationTests.Utils.DataHelpers.Workflows
{
    internal class WorkflowHelper
    {
        private readonly IDataSetClient _dataSets;
        private readonly IWorkflowsClient _workflows;

        public async Task<IWorkflow> GetAnyWorkflow()
        {
            var dataSets = await _dataSets.ListAsync();
            var workflows = await _workflows.ListAsync(dataSets.First().Id);

            return workflows.First();
        }

        public WorkflowHelper(IDataSetClient dataSets, IWorkflowsClient workflows)
        {
            _dataSets = dataSets;
            _workflows = workflows;
        }
    }
}
