using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake;
using IndicoV2.Jobs.Exceptions;
using Newtonsoft.Json.Linq;
using JobStatus = IndicoV2.Jobs.Models.JobStatus;

namespace IndicoV2.Jobs
{
    public class JobsClient : IJobsClient
    {
        private readonly IndicoStrawberryShakeClient _strawberryShake;
        private readonly IndicoClient _indicoClient;

        private readonly JobStatus[] _waitingForResult = {
            JobStatus.PENDING,
            JobStatus.RECEIVED,
            JobStatus.STARTED,
        };

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

        public async Task<string> GetResultAsync(string jobId, TimeSpan checkInterval, CancellationToken cancellationToken = default)
        {
            JobStatus status;
            while (_waitingForResult.Contains(status = await GetStatusAsync(jobId, cancellationToken)))
            {
                await Task.Delay(checkInterval, cancellationToken);
            }

            if (status != JobStatus.SUCCESS)
            {
                var failReason = await GetFailureReasonAsync(jobId);
                throw new JobNotSuccessfulException(status, failReason);
            }

            var result = await _strawberryShake.Jobs().GetResultAsync(jobId, cancellationToken);
            return result.Job.Result;
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
