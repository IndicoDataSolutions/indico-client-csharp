﻿using System;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Jobs;
using IndicoV2.Jobs.Models;
using IndicoV2.Submissions;
using IndicoV2.Submissions.Models;

namespace IndicoV2.Extensions.SubmissionResult
{
    internal class SubmissionResultAwaiter
    {
        private readonly ISubmissionsClient _submissionsClient;
        private readonly IJobsClient _jobsClient;

        public SubmissionResultAwaiter(ISubmissionsClient submissionsClient, IJobsClient jobsClient)
        {
            _submissionsClient = submissionsClient;
            _jobsClient = jobsClient;
        }

        public async Task<IJobResult> WaitReady(int submissionId, TimeSpan checkInterval, TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            using (var innerCancellationTokenSource = new CancellationTokenSource(timeout))
            using (cancellationToken.Register(() => innerCancellationTokenSource.Cancel(true)))
            {
                var innerTask = RepeatUntilReady(submissionId, checkInterval, innerCancellationTokenSource.Token);
                await await Task.WhenAny(innerTask, Task.Delay(-1, innerCancellationTokenSource.Token));

                return await innerTask;
            }
        }

        private async Task<IJobResult> RepeatUntilReady(int submissionId, TimeSpan checkInterval, CancellationToken cancellationToken)
        {
            while (SubmissionStatus.PROCESSING == (await _submissionsClient.GetAsync(submissionId, cancellationToken)).Status)
            {
                await Task.Delay(checkInterval, cancellationToken);
            }

            var jobId = await _jobsClient.GenerateSubmissionResultAsync(submissionId, cancellationToken);

            while (JobStatus.PENDING == await _jobsClient.GetStatusAsync(jobId, cancellationToken))
            {
                await Task.Delay(checkInterval, cancellationToken);
            }

            var jobResult = await _jobsClient.GetResult(jobId);

            return jobResult;
        }
    }
}
