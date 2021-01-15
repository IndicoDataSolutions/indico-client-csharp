using System;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Abstractions.Submissions;
using IndicoV2.Abstractions.Submissions.Models;

namespace IndicoV2.Submissions.LongRunning
{
    public class SubmissionAwaiter
    {
        private readonly ISubmissionsClient _submissionsClient;

        public SubmissionAwaiter(ISubmissionsClient submissionsClient)
        {
            _submissionsClient = submissionsClient;
        }

        public async Task<IJob> WaitReady(int submissionId, TimeSpan checkInterval, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var stopTime = GetStopTime(timeout);

            do
            {
                var submission = await _submissionsClient.GetAsync(submissionId, cancellationToken);

                if (submission.Status != SubmissionStatus.PROCESSING)
                {
                    return await _submissionsClient.GenerateSubmissionResult(submissionId, cancellationToken);
                }

                await Task.Delay(checkInterval, cancellationToken);
            } while (DateTime.Now < stopTime);

            throw new TimeoutException($"Timeout exceeded ({timeout}).");
        }

        private DateTime GetStopTime(TimeSpan timeout) =>
            timeout == TimeSpan.MaxValue ? DateTime.MaxValue : DateTime.Now + timeout;
    }
}
