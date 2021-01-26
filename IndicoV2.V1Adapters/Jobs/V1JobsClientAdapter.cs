using System;
using System.Threading;
using System.Threading.Tasks;
using Indico;
using Indico.Jobs;
using Indico.Mutation;
using IndicoV2.Jobs;
using IndicoV2.Jobs.Models;
using IndicoV2.V1Adapters.Submissions.Models;
using V1Types = Indico.Types;

namespace IndicoV2.V1Adapters.Jobs
{
    public class V1JobsClientAdapter : IJobsClient
    {
        private readonly IndicoClient _indicoClient;

        public V1JobsClientAdapter(IndicoClient indicoClient) => _indicoClient = indicoClient;


        public async Task<IJob> GenerateSubmissionResultAsync(int submissionId, CancellationToken cancellationToken)
        {
            var job = await new GenerateSubmissionResult(_indicoClient) { SubmissionId = submissionId }.Exec();

            return new V1JobAdapter(job);
        }

        public async Task<IJob> GetJobAsync(int submissionId, CancellationToken cancellationToken)
        {
            var job = await new SubmissionResult(_indicoClient) {SubmissionId = submissionId}.Exec();

            return new V1JobAdapter(job);
        }

        public async Task<IJobResult> GetResult(int jobId)
        {
            var jobResultV1 = await new Job(_indicoClient.GraphQLHttpClient, jobId.ToString()).Result();
            return new JobResultAdapter(jobResultV1);
        }

        public async Task<JobStatus> GetStatusAsync(int jobId, CancellationToken cancellationToken) =>
            Map(await new Job(_indicoClient.GraphQLHttpClient, jobId.ToString()).Status());

        private JobStatus Map(V1Types.JobStatus status) => (JobStatus)Enum.Parse(typeof(JobStatus), status.ToString());
    }
}
