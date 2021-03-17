using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Jobs;
using IndicoV2.Jobs.Exceptions;
using IndicoV2.Jobs.Models;
using IndicoV2.Models.Models;
using IndicoV2.V1Adapters.Models.Models;
using Newtonsoft.Json.Linq;

namespace IndicoV2.Extensions.Jobs
{
    public class JobAwaiter : IJobAwaiter
    {
        private readonly JobStatus[] _waitingForResult = {
            JobStatus.PENDING,
            JobStatus.RECEIVED,
            JobStatus.STARTED,
        };

        private readonly IJobsClient _jobsClient;


        public JobAwaiter(IJobsClient jobsClient) => _jobsClient = jobsClient;
        
        [Obsolete("Use generic version")]
        public async Task<JToken> WaitReadyAsync(string jobId, TimeSpan checkInterval, CancellationToken cancellationToken = default) =>
            await WaitReadyAsync<JToken>(jobId, checkInterval, cancellationToken);

        public async Task<IPredictionJobResult> WaitPredictionReadyAsync(string predictionJobId, TimeSpan checkInterval, CancellationToken cancellationToken) =>
            await WaitReadyAsync<PredictionJobResult>(predictionJobId, checkInterval, cancellationToken);

        public async Task<TResult> WaitReadyAsync<TResult>(string jobId, TimeSpan checkInterval, CancellationToken cancellationToken = default)
        {
            JobStatus status;

            while (_waitingForResult.Contains(status = await _jobsClient.GetStatusAsync(jobId, cancellationToken)))
            {
                await Task.Delay(checkInterval, cancellationToken);
            }

            if (status != JobStatus.SUCCESS)
            {
                var failReason = await _jobsClient.GetFailureReasonAsync(jobId);
                throw new JobNotSuccessfulException(status, failReason);
            }

            return await _jobsClient.GetResultAsync<TResult>(jobId, cancellationToken);
        }
    }
}
