using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IndicoV2.IntegrationTests.Utils.DataHelpers.Files;
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
        private readonly FileHelper _fileHelper;

        public SubmissionHelper(WorkflowHelper workflowHelper, FileHelper fileHelper, ISubmissionsClient submissions)
        {
            _workflowHelper = workflowHelper;
            _fileHelper = fileHelper;
            _submissions = submissions;
        }
        
        public async Task<ISubmission> GetAnyAsync()
        {
            await using var fileStream = _fileHelper.GetSampleFileStream();
            
            return await GetAnyAsync(fileStream);
        }

        public async Task<ISubmission> GetAnyAsync(Stream content)
        {
            var workflow = await _workflowHelper.GetAnyWorkflow();
            var submissionIds = await _submissions.CreateAsync(workflow.Id, new [] { ("csharp_test_content",content ?? throw new ArgumentNullException(nameof(content)))});
            var submission = await _submissions.GetAsync(submissionIds.Single());

            return submission;
        }

        public async Task<int> Get(IWorkflow workflow, Stream content) => (await _submissions.CreateAsync(workflow.Id, new[] { ("csharp_test_content", content) })).Single();

        public async Task<(int workflowId, int submissionId)> ListAnyAsync()
        {
            await using var content = _fileHelper.GetSampleFileStream();
            return await ListAnyAsync(content);
        }

        public async Task<(int workflowId, int submissionId)> ListAnyAsync(Stream content)
        {
            var workflow = await _workflowHelper.GetAnyWorkflow();
            var submissionIds = await _submissions.CreateAsync(workflow.Id, new[] {  ("csharp_test_content", content ?? throw new ArgumentNullException(nameof(content))) });

            return (workflow.Id, submissionIds.First());
        }
    }
}
