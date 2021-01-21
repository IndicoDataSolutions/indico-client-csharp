using System.IO;
using System.Linq;
using System.Threading.Tasks;

using IndicoV2.DataSets;
using IndicoV2.Submissions;
using IndicoV2.Submissions.Models;
using IndicoV2.Workflows;

namespace IndicoV2.IntegrationTests.Utils.DataHelpers.Submissions
{
    public class SubmissionHelper
    {
        private readonly IDataSetClient _dataSets;
        private readonly IWorkflowsClient _workflows;
        private readonly ISubmissionsClient _submissions;

        public async Task<ISubmission> GetAnyAsync(Stream content = null)
        {
            var dataSets = await _dataSets.ListAsync();
            var workflows = await _workflows.ListAsync(dataSets.First().Id);
            // TODO: can be replaced with fetching first of existing
            var submissionIds = await _submissions.CreateAsync(workflows.First().Id, new[] { content ?? new MemoryStream() });
            var submission = await _submissions.GetAsync(submissionIds.Single());

            return submission;
        }

        public async Task<(int workflowId, int submissionId)> ListAnyAsync(Stream content = null)
        {
            var dataSets = await _dataSets.ListAsync();
            int dataSetId = dataSets.First().Id;
            var workflows = await _workflows.ListAsync(dataSetId);
            int workflowId = workflows.First().Id;
            var submissionIds = await _submissions.CreateAsync(workflowId, new[] { content ?? new MemoryStream() });

            return (workflowId, submissionIds.First());
        }

        public SubmissionHelper(IDataSetClient dataSets, IWorkflowsClient workflows, ISubmissionsClient submissions)
        {
            _dataSets = dataSets;
            _workflows = workflows;
            _submissions = submissions;
        }
    }
}
