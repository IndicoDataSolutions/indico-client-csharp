using System;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Abstractions.Submissions;
using IndicoV2.Abstractions.Submissions.Models;
using IndicoV2.Submissions.LongRunning;

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
