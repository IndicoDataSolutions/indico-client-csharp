using System;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Submissions.LongRunning;
using IndicoV2.Submissions.Models;

namespace IndicoV2.Submissions
{
    public static partial class SubmissionClientExtensions
    {
        public static Task<IJob> GetJobWhenReady(this ISubmissionsClient submissionsClient, int submissionId,
            TimeSpan? checkInterval = default, TimeSpan? timeout = default,
            CancellationToken cancellationToken = default) =>
            new SubmissionAwaiter(submissionsClient).WaitReady(submissionId, checkInterval ?? TimeSpan.FromSeconds(1),
                timeout ?? TimeSpan.Zero, cancellationToken);
    }
}
