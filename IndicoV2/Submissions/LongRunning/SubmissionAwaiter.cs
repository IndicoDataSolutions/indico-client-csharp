using System;
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

        public async Task<IJob> WaitReady(int submissionId, TimeSpan checkInterval, TimeSpan timeout)
        {
            var stopTime = DateTime.Now + timeout;
            do
            {
                var submission = await _submissionsClient.GetAsync(submissionId);

                if (submission.Status != SubmissionStatus.PROCESSING)
                {
                    return await _submissionsClient.GenerateSubmissionResult(submissionId);
                }

                await Task.Delay(checkInterval);
            } while (DateTime.Now < stopTime);

            throw new TimeoutException($"Timeout exceeded ({timeout}).");
        }
    }
}
