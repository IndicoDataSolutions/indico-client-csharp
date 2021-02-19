using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Extensions.JobResultBuilders;
using IndicoV2.Extensions.Jobs;
using IndicoV2.Storage;
using IndicoV2.Submissions;
using IndicoV2.Submissions.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IndicoV2.Extensions.SubmissionResult
{
    internal class SubmissionResultAwaiter : ISubmissionResultAwaiter
    {
        private readonly ISubmissionsClient _submissionsClient;
        private readonly IJobAwaiter _jobAwaiter;
        private readonly IStorageClient _storageClient;
        private readonly JobResultBuilder _jobResultBuilder = new JobResultBuilder();

        public SubmissionResultAwaiter(ISubmissionsClient submissionsClient, IJobAwaiter jobAwaiter, IStorageClient storageClient)
        {
            _submissionsClient = submissionsClient;
            _jobAwaiter = jobAwaiter;
            _storageClient = storageClient;
        }

        public async Task<JObject> WaitReady(int submissionId, TimeSpan checkInterval, CancellationToken cancellationToken)
            => await WaitReady(s => s != SubmissionStatus.PROCESSING, submissionId, checkInterval, cancellationToken);

        public async Task<JObject> WaitReady(int submissionId, SubmissionStatus awaitedStatus, TimeSpan checkInterval = default, CancellationToken cancellationToken = default)
            => await WaitReady(s => s == awaitedStatus, submissionId, checkInterval, cancellationToken);

        private async Task<JObject> WaitReady(Predicate<SubmissionStatus> isAwaitedStatus, int submissionId, TimeSpan checkInterval, CancellationToken cancellationToken)
        {
            ISubmission submission;

            while (!isAwaitedStatus((submission = await _submissionsClient.GetAsync(submissionId, cancellationToken)).Status))
            {
                await Task.Delay(checkInterval, cancellationToken);
            }

            var resultUri = $"indico-file://{submission.ResultFile}";
            var result = await _storageClient.GetAsync(new Uri(resultUri));

            using (var reader = new JsonTextReader(new StreamReader(result)))
            {
                return JsonSerializer.Create().Deserialize<JObject>(reader);
            }
        }
    }
}
