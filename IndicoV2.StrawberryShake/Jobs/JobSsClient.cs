using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace IndicoV2.StrawberryShake.Jobs
{
    public class JobSsClient : ErrorHandlingWrapper
    {
        private readonly ServiceProvider _services;

        public JobSsClient(ServiceProvider services) => _services = services;

        public async Task<IJobStatusResult> GetStatusAsync(string jobId, CancellationToken cancellationToken)
        {
            var result = await ExecuteAsync(() => _services
            .GetRequiredService<JobStatusQuery>().ExecuteAsync(jobId, cancellationToken));
            return result;
        }

        public async Task<IJobStatusResult> GetResultAsync(string jobId, CancellationToken cancellationToken)
        {
            var result = await ExecuteAsync(() => _services
            .GetRequiredService<JobStatusQuery>().ExecuteAsync(jobId, cancellationToken));
            return result;
        }

        public async Task<IJobStatusResult> GetFailureReasonAsync(string jobId, CancellationToken cancellationToken)
        {
            var result = await ExecuteAsync(() => _services
            .GetRequiredService<JobStatusQuery>().ExecuteAsync(jobId, cancellationToken));
            return result;
        }
    }
}
