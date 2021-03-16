using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace IndicoV2.Extensions.Jobs
{
    public interface IJobAwaiter
    {
        /// <summary>
        /// Wait until Job is finished.
        /// </summary>
        /// <param name="jobId">Job's Id</param>
        /// <param name="checkInterval">Interval between requests.</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns></returns>
        Task<TResult> WaitReadyAsync<TResult>(string jobId, TimeSpan checkInterval, CancellationToken cancellationToken = default);

        /// <summary>
        /// Wait until Job is finished.
        /// </summary>
        /// <param name="jobId">Job's Id</param>
        /// <param name="checkInterval">Interval between requests.</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns></returns>
        [Obsolete("Use generic version of this method.")]
        Task<JToken> WaitReadyAsync(string jobId, TimeSpan checkInterval, CancellationToken cancellationToken = default);
    }
}