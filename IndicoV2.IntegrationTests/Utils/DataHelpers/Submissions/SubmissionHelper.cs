using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IndicoV2.IntegrationTests.Utils.DataHelpers.Files;
using IndicoV2.IntegrationTests.Utils.DataHelpers.Workflows;
using IndicoV2.Submissions;
using IndicoV2.Submissions.Models;

namespace IndicoV2.IntegrationTests.Utils.DataHelpers.Submissions
{
    internal class SubmissionHelper
    {
        private readonly WorkflowHelper _workflowHelper;
        private readonly ISubmissionsClient _submissions;
        private readonly FileHelper _fileHelper;

        public async Task<ISubmission> GetAnyAsync()
        {
            await using var fileStream = await _fileHelper.GetSampleFileStream();
            
            return await GetAnyAsync(fileStream);
        }

        public async Task<ISubmission> GetAnyAsync(Stream content)
        {
            var workflow = await _workflowHelper.GetAnyWorkflow();
            var submissionIds = await _submissions.CreateAsync(workflow.Id, new [] { content ?? throw new ArgumentNullException(nameof(content))});
            var submission = await _submissions.GetAsync(submissionIds.Single());

            return submission;
        }

        public SubmissionHelper(WorkflowHelper workflowHelper, FileHelper fileHelper, ISubmissionsClient submissions)
        {
            _workflowHelper = workflowHelper;
            _fileHelper = fileHelper;
            _submissions = submissions;
        }
    }
}
