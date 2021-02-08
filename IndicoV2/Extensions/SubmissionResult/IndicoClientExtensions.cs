using System;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Extensions.SubmissionResult;
using IndicoV2.Jobs.Models.Results;
using IndicoV2.Submissions.Models;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        /// <summary>
        /// Waits until given <seealso cref="ISubmission"/> gets processed by the server and then returns <see cref="ISubmission"/>'s <see cref="IJobResult"/>.
        /// Throws <see cref="TaskCanceledException"/> after <paramref name="cancellationToken"/> has been cancelled or <paramref name="timeout"/> has been exceeded.
        /// </summary>
        /// <param name="checkInterval">Interval between <see cref="ISubmission"/> status checks requests.</param>
        /// <exception cref="TaskCanceledException"></exception>
        public static Task<IUrlJobResult> GetResultWhenReady(this IndicoClient indicoClient, int submissionId,
            TimeSpan? checkInterval = default, TimeSpan? timeout = default,
            CancellationToken cancellationToken = default) =>
            new SubmissionResultAwaiter(indicoClient.Submissions(), indicoClient.Jobs()).WaitReady(submissionId, checkInterval ?? TimeSpan.FromSeconds(1),
                timeout ?? TimeSpan.Zero, cancellationToken);
    }
}
