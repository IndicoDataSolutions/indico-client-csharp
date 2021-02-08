using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Indico;
using Indico.Mutation;
using Indico.Query;
using IndicoV2.Submissions;
using IndicoV2.Submissions.Models;
using IndicoV2.V1Adapters.Converters;
using IndicoV2.V1Adapters.Submissions.Models;

namespace IndicoV2.V1Adapters.Submissions
{
    public class SubmissionsV1ClientAdapter : ISubmissionsClient
    {
        private readonly IndicoClient _indicoClient;

        public SubmissionsV1ClientAdapter(IndicoClient indicoClient) => _indicoClient = indicoClient;

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<Stream> streams, CancellationToken cancellationToken = default)
        {
            var submissionMutation = new WorkflowSubmission(_indicoClient) { Streams = streams.ToList(), WorkflowId = workflowId };
            // TODO: handle cancellation token
            var submissionIds = await submissionMutation.Exec(cancellationToken);

            return submissionIds;
        }

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<Uri> uris, CancellationToken cancellationToken = default)
        {
            var submissionMutation = new WorkflowSubmission(_indicoClient) { WorkflowId = workflowId, Urls = uris.Select(u => u.ToString()).ToList() };
            var submissionIds = await submissionMutation.Exec(cancellationToken);

            return submissionIds;
        }

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<string> paths, CancellationToken cancellationToken = default)
        {
            var submissionMutation = new WorkflowSubmission(_indicoClient) { WorkflowId = workflowId, Files = paths.ToList() };
            var submissionIds = await submissionMutation.Exec(cancellationToken);

            return submissionIds;
        }

        public async Task<IEnumerable<ISubmission>> ListAsync(IEnumerable<int> submissionIds, IEnumerable<int> workflowIds, IFilter filters, int limit = 1000, CancellationToken cancellationToken = default)
        {
            var listSubmissionQuery = new ListSubmissions(_indicoClient)
            {
                SubmissionIds = submissionIds?.ToList(),
                WorkflowIds = workflowIds?.ToList(),
                Filters = filters != null ? filters.ConvertToLegacy() : new Indico.Entity.SubmissionFilter(),
                Limit = limit
            };

            var submissions = await listSubmissionQuery.Exec(cancellationToken);

            return submissions.Select(s => new V1SubmissionAdapter(s));
        }

        public async Task<ISubmission> GetAsync(int submissionId, CancellationToken cancellationToken = default)
        {
            var submission = await new GetSubmission(_indicoClient) { Id = submissionId }.Exec(cancellationToken);
            return new V1SubmissionAdapter(submission);
        }
    }
}
