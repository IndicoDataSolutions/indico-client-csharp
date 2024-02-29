using System;
using System.Collections.Generic;
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

        public async Task<ISubmission> GetAnyAsync(int workflow_id)
        {
            await using var fileStream = _fileHelper.GetSampleFileStream();

            return await GetAnyAsync(fileStream, workflow_id);
        }

        public async Task<ISubmission> GetAnyAsync(Stream content, int workflow_id)
        {
            var submissionIds = await _submissions.CreateAsync(workflow_id, new [] { ("csharp_test_content",content ?? throw new ArgumentNullException(nameof(content)))});
            var submission = await _submissions.GetAsync(submissionIds.Single());

            return submission;
        }

        public async Task<int> Get(IWorkflow workflow, Stream content) => (await _submissions.CreateAsync(workflow.Id, new[] { ("csharp_test_content", content) })).Single();

        public async Task<ISubmission> Get(int workflowId, SubmissionResultsFileVersion? resultsFileVersion = null, bool bundle = false)
        {
            var submissionIds = await _submissions.CreateAsync(workflowId, paths: new[] { _fileHelper.GetSampleFilePath() }, resultsFileVersion: resultsFileVersion, bundle: bundle);
            var submission = await _submissions.GetAsync(submissionIds.Single());

            return submission;
        }

        public async Task<(int workflowId, int submissionId)> ListAnyAsync(int workflowId)
        {
            await using var content = _fileHelper.GetSampleFileStream();
            return await ListAnyAsync(content, workflowId);
        }

        public async Task<(int workflowId, int submissionId)> ListAnyAsync(Stream content, int workflowId)
        {
            var submissionIds = await _submissions.CreateAsync(workflowId, new[] {  ("csharp_test_content", content ?? throw new ArgumentNullException(nameof(content))) });

            return (workflowId, submissionIds.First());
        }


        public async Task<IEnumerable<ISubmission>> ListAsync(IEnumerable<int> submissionIds, int limit = 1000)
        {
            var submissions = await _submissions.ListAsync(submissionIds, new List<int>(), null, limit: limit);

            return submissions;
        }
    }
}
