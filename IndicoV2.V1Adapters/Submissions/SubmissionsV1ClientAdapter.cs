using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Indico;
using Indico.Mutation;
using Indico.Query;
using IndicoV2.Abstractions.Submissions;
using IndicoV2.Abstractions.Submissions.Models;
using IndicoV2.V1Adapters.Submissions.Models;

namespace IndicoV2.V1Adapters.Submissions
{
    public class SubmissionsV1ClientAdapter : ISubmissionsClient
    {
        private readonly IndicoClient _indicoClient;

        public SubmissionsV1ClientAdapter(IndicoClient indicoClient)
        {
            this._indicoClient = indicoClient;
        }

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, Stream[] streams, CancellationToken cancellationToken = default)
        {
            var submissionMutation = new WorkflowSubmission(_indicoClient) { Streams = streams.ToList(), WorkflowId = workflowId };
            // TODO: handle cancellation token
            var submissionIds = await submissionMutation.Exec();

            return submissionIds;
        }

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, Uri[] uri, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, string[] paths, CancellationToken cancellationToken = default)
        {
            var submissionMutation = new WorkflowSubmission(_indicoClient) {WorkflowId = workflowId, Files = paths.ToList()};
            var submissionIds = await submissionMutation.Exec();

            return submissionIds;
        }

        public async Task<ISubmission> GetAsync(int submissionId)
        {
            var submission = await new GetSubmission(_indicoClient) {Id = submissionId}.Exec();
            return new V1SubmissionAdapter(submission);
        }

        public async Task<IJob> GenerateSubmissionResult(int submissionId)
        {
            var job = await new GenerateSubmissionResult(_indicoClient) {SubmissionId = submissionId}.Exec();

            return new V1JobAdapter(job);
        }

        public async Task<IJob> GetJobAsync(int submissionId, CancellationToken cancellationToken)
        {
            var job = await new SubmissionResult(_indicoClient) { SubmissionId = submissionId }.Exec();
            // TODO: handle cancellation token

            return new V1JobAdapter(job);
        }
    }
}
