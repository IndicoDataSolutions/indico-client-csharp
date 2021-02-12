﻿using System;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Jobs;
using IndicoV2.Jobs.Exceptions;
using IndicoV2.Jobs.Models;
using Newtonsoft.Json.Linq;

namespace IndicoV2.Extensions.Jobs
{
    public class JobAwaiter : IJobAwaiter
    {
        private readonly IJobsClient _jobsClient;


        public JobAwaiter(IJobsClient jobsClient) => _jobsClient = jobsClient;


        public async Task<JToken> WaitReadyAsync(string jobId, TimeSpan checkInterval, CancellationToken cancellationToken = default)
        {
            JobStatus status;

            while (JobStatus.PENDING == (status = await _jobsClient.GetStatusAsync(jobId, cancellationToken)))
            {
                await Task.Delay(checkInterval, cancellationToken);
            }

            if (status != JobStatus.SUCCESS)
            {
                throw new JobNotSuccessfulException(status);
            }

            return await _jobsClient.GetResultAsync(jobId);
        }
    }
}