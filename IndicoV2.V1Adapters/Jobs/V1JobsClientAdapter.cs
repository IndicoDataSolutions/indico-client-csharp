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

        public async Task<string> GenerateSubmissionResultAsync(int submissionId, CancellationToken cancellationToken)
        {
            var job = await new GenerateSubmissionResult(_indicoClient) { SubmissionId = submissionId }.Exec();

            return job.Id;
        }

        public async Task<JToken> GetResultAsync(string jobId) => await new Job(_indicoClient.GraphQLHttpClient, jobId).Result();

        public async Task<JobStatus> GetStatusAsync(string jobId, CancellationToken cancellationToken) =>
            _jobStatusConverter.Map(await new Job(_indicoClient.GraphQLHttpClient, jobId).Status());

        private JobStatus Map(V1Types.JobStatus status) => (JobStatus)Enum.Parse(typeof(JobStatus), status.ToString());
    }
}
