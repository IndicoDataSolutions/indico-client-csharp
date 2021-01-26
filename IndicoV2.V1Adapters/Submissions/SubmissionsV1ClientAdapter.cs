﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Indico;
using Indico.Mutation;
using Indico.Query;
using IndicoV2.Jobs.Models;
using IndicoV2.Submissions;
using IndicoV2.Submissions.Models;
using IndicoV2.V1Adapters.Submissions.Models;

namespace IndicoV2.V1Adapters.Submissions
{
    public class SubmissionsV1ClientAdapter : ISubmissionsClient
    {
        private readonly IndicoClient _indicoClient;

        public SubmissionsV1ClientAdapter(IndicoClient indicoClient) => _indicoClient = indicoClient;

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, Stream[] streams, CancellationToken cancellationToken = default)
        {
            var submissionMutation = new WorkflowSubmission(_indicoClient) { Streams = streams.ToList(), WorkflowId = workflowId };
            // TODO: handle cancellation token
            var submissionIds = await submissionMutation.Exec();

            return submissionIds;
        }

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, string[] paths, CancellationToken cancellationToken)
        {
            var submissionMutation = new WorkflowSubmission(_indicoClient) {WorkflowId = workflowId, Files = paths.ToList()};
            var submissionIds = await submissionMutation.Exec();

            return submissionIds;
        }

        public async Task<ISubmission> GetAsync(int submissionId, CancellationToken cancellationToken = default)
        {
            var submission = await new GetSubmission(_indicoClient) {Id = submissionId}.Exec();
            return new V1SubmissionAdapter(submission);
        }
    }
}
