using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Jobs.Models;

namespace IndicoV2.Jobs
{
    public interface IJobsClient
    {
        /// <summary>
        /// Gets Job's status
        /// </summary>
        /// <param name="jobId">Job's Id</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns>Job's Status</returns>
        Task<JobStatus> GetStatusAsync(string jobId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets Job's result
        /// </summary>
        /// <param name="jobId">Job's Id</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns>Job result</returns>
        Task<string> GetResultAsync(string jobId, CancellationToken cancellationToken = default);

        Task<string> GetFailureReasonAsync(string jobId);
    }
}
