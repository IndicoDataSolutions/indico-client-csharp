using System;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Submissions.Models;
using Newtonsoft.Json.Linq;

namespace IndicoV2.Extensions.SubmissionResult
{
    public interface ISubmissionResultAwaiter
    {
        /// <summary>
        /// Waits until given <seealso cref="ISubmission"/> gets processed by the server and then returns <see cref="ISubmission"/>'s result.
        /// Throws <see cref="TaskCanceledException"/> after <paramref name="cancellationToken"/> has been cancelled or <paramref name="timeout"/> has been exceeded.
        /// </summary>
        /// <param name="submissionId"><seealso cref="ISubmission"/>'s Id</param>
        /// <param name="checkInterval">Interval between server calls.</param>
        /// <param name="timeout">Maximum wait time for the result.</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <exception cref="TaskCanceledException"></exception>
        Task<JObject> WaitReady(int submissionId, TimeSpan checkInterval = default, TimeSpan timeout = default,
            CancellationToken cancellationToken = default);

        /// <inheritdoc cref="WaitReady(int,System.TimeSpan,System.TimeSpan,System.Threading.CancellationToken)"/>
        /// <param name="awaitedStatus">Wait until submission reaches this status.</param>
        Task<JObject> WaitReady(int submissionId, SubmissionStatus awaitedStatus, TimeSpan checkInterval = default, TimeSpan timeout = default,
            CancellationToken cancellationToken = default);
    }
}