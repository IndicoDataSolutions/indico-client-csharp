using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using Indico;
using Indico.Jobs;
using IndicoV2.Jobs;
using IndicoV2.Jobs.Models;
using Newtonsoft.Json.Linq;

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

        public async Task<JToken> GetResultAsync(string jobId, CancellationToken cancellationToken) =>
            await GetResultAsync<JObject>(jobId, cancellationToken);
        
        public async Task<TResult> GetResultAsync<TResult>(string jobId, CancellationToken cancellationToken = default)
        {
            var jobQuery = new Job(_indicoClient.GraphQLHttpClient, jobId);
            var result = await jobQuery.Result<TResult>(cancellationToken);

            return result;
        }

        public async Task<string> GetFailureReasonAsync(string jobId)
        {
            var queryString = @"
                    query GetJob($id: String!) {
                        job(id: $id) {
                            id
                            status
                            result
                        }
                    }";
            var query = new GraphQL.GraphQLRequest()
            {
                Query = queryString,
                OperationName = "GetJob",
                Variables = new { id = jobId },
            };
            var jobResponse = await _indicoClient.GraphQLHttpClient.SendQueryAsync<dynamic>(query);
            var job = (JObject)jobResponse.Data.job;

            var jobStatus = (JobStatus)Enum.Parse(typeof(JobStatus), job.Value<string>("status"));
            
            if (jobStatus != JobStatus.FAILURE)
            {
                throw new InvalidOperationException(
                    $"Cannot get failure reason, the job has not failed (status: {jobStatus}");
            }
            var failReason = job.Value<string>("result");
            var failReasonObject = JObject.Parse(failReason);
            failReason = failReasonObject.Value<string>("message");

            return failReason;
        }

        public async Task<JobStatus> GetStatusAsync(string jobId, CancellationToken cancellationToken) =>
            _jobStatusConverter.Map(await new Job(_indicoClient.GraphQLHttpClient, jobId).Status());
    }
}
