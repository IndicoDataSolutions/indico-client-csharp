using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake;
using Newtonsoft.Json.Linq;
using JobStatus = IndicoV2.Jobs.Models.JobStatus;
using Newtonsoft.Json;

namespace IndicoV2.Jobs
{
    public class JobsClient : IJobsClient
    {
        private readonly IndicoStrawberryShakeClient _strawberryShake;
        private readonly IndicoClient _indicoClient;

        public JobsClient(IndicoClient indicoClient)
        {
            _indicoClient = indicoClient;
            _strawberryShake = indicoClient.IndicoStrawberryShakeClient;
        }

        public async Task<JobStatus> GetStatusAsync(string jobId, CancellationToken cancellationToken = default)
        {
            var result = await _strawberryShake.Jobs().GetStatusAsync(jobId, cancellationToken);
            return (JobStatus)result.Job.Status.Value;
        }

        public async Task<TResult> GetResultAsync<TResult>(string jobId, CancellationToken cancellationToken = default)
        {
            var result = await _strawberryShake.Jobs().GetResultAsync(jobId, cancellationToken);
            // use json decoder to decode result into whatever tresult would be
            return JsonConvert.DeserializeObject<TResult>(result.Job.Result);
        }

        public async Task<string> GetFailureReasonAsync(string jobId)
        {
            var result = await _strawberryShake.Jobs().GetFailureReasonAsync(jobId, default);
            var jobStatus = result.Job.Status;
            if ((JobStatus)jobStatus != JobStatus.FAILURE)
            {
                throw new InvalidOperationException(
                    $"Cannot get failure reason, the job has not failed (status: {jobStatus}");
            }
            var failReason = result.Job.Result;
            var failReasonObject = JObject.Parse(failReason);
            failReason = failReasonObject.Value<string>("message");

            return failReason;
        }
    }
}
