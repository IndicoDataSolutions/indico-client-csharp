using System;
using System.Threading;
using System.Threading.Tasks;
using Indico;
using Indico.Jobs;
using Indico.Mutation;
using IndicoV2.Jobs;
using IndicoV2.Jobs.Models;
using Newtonsoft.Json.Linq;
using V1Types = Indico.Types;

namespace IndicoV2.V1Adapters.Jobs
{
    public class V1JobsClientAdapter : IJobsClient
    {
        private readonly IndicoClient _indicoClient;
        private readonly JobStatusConverter _jobStatusConverter;

        public V1JobsClientAdapter(IndicoClient indicoClient, JobStatusConverter jobStatusConverter)
        {
            _indicoClient = indicoClient;
            _jobStatusConverter = jobStatusConverter;
        }

        public async Task<Guid> GenerateSubmissionResultAsync(int submissionId, CancellationToken cancellationToken)
        {
            var job = await new GenerateSubmissionResult(_indicoClient) { SubmissionId = submissionId }.Exec();

            return Guid.Parse(job.Id);
        }

        public async Task<JToken> GetResult(Guid jobId) => await new Job(_indicoClient.GraphQLHttpClient, jobId.ToString()).Result();

        public async Task<JobStatus> GetStatusAsync(Guid jobId, CancellationToken cancellationToken) =>
            _jobStatusConverter.Map(await new Job(_indicoClient.GraphQLHttpClient, jobId.ToString()).Status());

        private JobStatus Map(V1Types.JobStatus status) => (JobStatus)Enum.Parse(typeof(JobStatus), status.ToString());
    }
}
