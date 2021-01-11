using System;
using Indico;
using Indico.Mutation;
using IndicoV2.Client.Submissions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Client.Submissions.Models;
using IndicoV2.V1Adapters.Client.Submissions.Models;

namespace IndicoV2.V1Adapters.Client.Submissions
{
    public class IndicoSubmissionsAdapter : ISubmissionsClient
    {
        private readonly IndicoClient indicoClient;

        public IndicoSubmissionsAdapter(IndicoClient indicoClient)
        {
            this.indicoClient = indicoClient;
        }

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, Stream[] streams, CancellationToken cancellationToken)
        {
            var submissionMutation = new WorkflowSubmission(indicoClient) { Streams = streams.ToList(), WorkflowId = workflowId };
            // TODO: handle cancellation token
            var submissionIds = await submissionMutation.Exec();

            return submissionIds;
        }

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, Uri[] uri, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<Job> GetJobAsync(int submissionId, CancellationToken cancellationToken)
        {
            var job = await new SubmissionResult(indicoClient) {SubmissionId = submissionId}.Exec();
            // TODO: handle cancellation token

            return new V1JobAdapter(job);
        }
    }
}
