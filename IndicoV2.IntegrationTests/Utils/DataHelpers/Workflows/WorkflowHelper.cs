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

        public WorkflowHelper(IDataSetClient dataSets, IWorkflowsClient workflows)
        {
            _dataSets = dataSets;
            _workflows = workflows;
        }

        public async Task<IWorkflow> GetAnyWorkflow()
        {
            var workflows = await _workflows.ListAsync(null, default);

            return workflows.First(wf => wf.ReviewEnabled);
        }
    }
}
