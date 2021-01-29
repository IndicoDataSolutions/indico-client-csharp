using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IndicoV2.IntegrationTests.Utils.DataHelpers.Workflows;
using IndicoV2.Submissions;
using IndicoV2.Submissions.Models;
using IndicoV2.Workflows.Models;

namespace IndicoV2.IntegrationTests.Utils.DataHelpers.Submissions
{
    internal class SubmissionHelper
    {
        private readonly WorkflowHelper _workflowHelper;
        private readonly ISubmissionsClient _submissions;

        public SubmissionHelper(WorkflowHelper workflowHelper, ISubmissionsClient submissions)
        {
            _workflowHelper = workflowHelper;
            _submissions = submissions;
        }

        public async Task<ISubmission> GetAnyAsync() => await GetAnyAsync(new MemoryStream(new byte[3]));

        public async Task<ISubmission> GetAnyAsync(Stream content)
        {
            var workflow = await _workflowHelper.GetAnyWorkflow();
            var submissionIds = await _submissions.CreateAsync(workflow.Id, new[] { content ?? new MemoryStream() });
            var submission = await _submissions.GetAsync(submissionIds.Single());

            return submission;
        }

        public async Task<int> Get(IWorkflow workflow, Stream content) => (await _submissions.CreateAsync(workflow.Id, new[] { content })).Single();

        public async Task<(int workflowId, int submissionId)> ListAnyAsync(Stream content = null)
        {
            var workflow = await _workflowHelper.GetAnyWorkflow();
            var submissionIds = await _submissions.CreateAsync(workflow.Id, new[] { content ?? new MemoryStream() });

            return (workflow.Id, submissionIds.First());
        }
    }
}
