using System;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Jobs.Models.Results;
using IndicoV2.Submissions.Models;
using Newtonsoft.Json.Linq;

namespace IndicoV2.Extensions.SubmissionResult
{
    public interface ISubmissionResultAwaiter
    {
        /// <summary>
        /// Waits until given <seealso cref="ISubmission"/> gets processed by the server and then returns <see cref="ISubmission"/>'s <see cref="IJobResult"/>.
        /// Throws <see cref="TaskCanceledException"/> after <paramref name="cancellationToken"/> has been cancelled or <paramref name="timeout"/> has been exceeded.
        /// </summary>
        /// <param name="checkInterval">Interval between <see cref="ISubmission"/> status checks requests.</param>
        /// <exception cref="TaskCanceledException"></exception>
        Task<JObject> WaitReady(int submissionId, TimeSpan checkInterval = default, TimeSpan timeout = default,
            CancellationToken cancellationToken = default);
    }
}