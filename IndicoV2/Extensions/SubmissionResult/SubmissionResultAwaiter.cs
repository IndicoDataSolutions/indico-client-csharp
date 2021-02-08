﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Extensions.JobResultBuilders;
using IndicoV2.Jobs;
using IndicoV2.Jobs.Models;
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
        private readonly IJobsClient _jobsClient;
        private readonly IStorageClient _storageClient;
        private readonly JobResultBuilder _jobResultBuilder = new JobResultBuilder();

        public SubmissionResultAwaiter(ISubmissionsClient submissionsClient, IJobsClient jobsClient, IStorageClient storageClient)
        {
            _submissionsClient = submissionsClient;
            _jobsClient = jobsClient;
            _storageClient = storageClient;
        }

        public async Task<JObject> WaitReady(int submissionId, TimeSpan checkInterval, TimeSpan timeout,
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

        private async Task<JObject> RepeatUntilReady(int submissionId, TimeSpan checkInterval, CancellationToken cancellationToken)
        {
            SubmissionStatus submissionStatus;

            while (SubmissionStatus.PROCESSING == (submissionStatus = (await _submissionsClient.GetAsync(submissionId, cancellationToken)).Status))
            {
                await Task.Delay(checkInterval, cancellationToken);
            }

            if (submissionStatus == SubmissionStatus.FAILED)
            {
                // TODO: throw
            }

            var jobId = await _jobsClient.GenerateSubmissionResultAsync(submissionId, cancellationToken);
            JobStatus jobStatus;

            while (JobStatus.PENDING == (jobStatus = await _jobsClient.GetStatusAsync(jobId, cancellationToken)))
            {
                await Task.Delay(checkInterval, cancellationToken);
            }

            if (jobStatus == JobStatus.FAILURE)
            {
                // TODO: throw
            }

            var jobResultJson = await _jobsClient.GetResultAsync(jobId);
            var jobResult = _jobResultBuilder.GetSubmissionJobResult((JObject)jobResultJson);

            var result = await _storageClient.GetAsync(jobResult.Url);
                
            using (var reader = new JsonTextReader(new StreamReader(result)))
            {
                return JsonSerializer.Create().Deserialize<JObject>(reader);
            }
        }
    }
}
