﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Extensions.JobResultBuilders;
using IndicoV2.Extensions.Jobs;
using IndicoV2.Jobs;
using IndicoV2.Storage;
using IndicoV2.Submissions;
using IndicoV2.Submissions.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IndicoV2.Extensions.SubmissionResult
{
    internal class SubmissionResultAwaiter : ISubmissionResultAwaiter
    {
        private readonly ISubmissionsClient _submissionsClient;
        private readonly IJobAwaiter _jobAwaiter;
        private readonly IJobsClient _jobsClient;
        private readonly IStorageClient _storageClient;
        private readonly JobResultBuilder _jobResultBuilder = new JobResultBuilder();

        public SubmissionResultAwaiter(ISubmissionsClient submissionsClient, IJobsClient jobsClient, IJobAwaiter jobAwaiter, IStorageClient storageClient)
        {
            _submissionsClient = submissionsClient;
            _jobAwaiter = jobAwaiter;
            _jobsClient = jobsClient;
            _storageClient = storageClient;
        }

        public Task<JObject> WaitReady(int submissionId, TimeSpan checkInterval,
            TimeSpan timeout, CancellationToken cancellationToken)
            => WaitReady(s => s != SubmissionStatus.PROCESSING, submissionId, checkInterval, timeout, cancellationToken);

        public Task<JObject> WaitReady(int submissionId, SubmissionStatus awaitedStatus,
            TimeSpan checkInterval = default, TimeSpan timeout = default, CancellationToken cancellationToken = default)
            => WaitReady(s => s == awaitedStatus, submissionId, checkInterval, timeout, cancellationToken);

        private async Task<JObject> WaitReady(Predicate<SubmissionStatus> isExpectedStatus, int submissionId, TimeSpan checkInterval, TimeSpan timeout, CancellationToken cancellationToken)
        {
            using (var innerCancellationTokenSource = new CancellationTokenSource(timeout))
            using (cancellationToken.Register(() => innerCancellationTokenSource.Cancel(true)))
            {
                var innerTask = RepeatUntilReady(isExpectedStatus, submissionId, checkInterval, innerCancellationTokenSource.Token);
                await await Task.WhenAny(innerTask, Task.Delay(-1, innerCancellationTokenSource.Token));

                return await innerTask;
            }
        }

        private async Task<JObject> RepeatUntilReady(Predicate<SubmissionStatus> isAwaitedStatus, int submissionId, TimeSpan checkInterval, CancellationToken cancellationToken)
        {
            while (!isAwaitedStatus((await _submissionsClient.GetAsync(submissionId, cancellationToken)).Status))
            {
                await Task.Delay(checkInterval, cancellationToken);
            }

            var jobId = await _jobsClient.GenerateSubmissionResultAsync(submissionId, cancellationToken);
            var jobResultJson = await _jobAwaiter.WaitReadyAsync(jobId, checkInterval, cancellationToken);
            var jobResult = _jobResultBuilder.GetSubmissionJobResult((JObject)jobResultJson);

            var result = await _storageClient.GetAsync(jobResult.Url);

            using (var reader = new JsonTextReader(new StreamReader(result)))
            {
                return JsonSerializer.Create().Deserialize<JObject>(reader);
            }
        }
    }
}
