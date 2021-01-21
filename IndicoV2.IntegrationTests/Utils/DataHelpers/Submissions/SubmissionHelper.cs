using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IndicoV2.IntegrationTests.Utils.DataHelpers.Workflows;
using IndicoV2.Submissions;
using IndicoV2.Submissions.Models;

namespace IndicoV2.IntegrationTests.Utils.DataHelpers.Submissions
{
    internal class SubmissionHelper
    {
        private readonly WorkflowHelper _workflowHelper;
        private readonly ISubmissionsClient _submissions;

        public async Task<ISubmission> GetAnyAsync(Stream content = null)
        {
            var workflow = await _workflowHelper.GetAnyWorkflow();
            var submissionIds = await _submissions.CreateAsync(workflow.Id, new [] { content ?? new MemoryStream()});
            var submission = await _submissions.GetAsync(submissionIds.Single());

            return submission;
        }

        public SubmissionHelper(WorkflowHelper workflowHelper, ISubmissionsClient submissions)
        {
            _workflowHelper = workflowHelper;
            _submissions = submissions;
        }
    }
}
