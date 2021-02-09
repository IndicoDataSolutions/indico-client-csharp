using System.Threading;
using System.Threading.Tasks;
using Indico.Exception;
using Indico.Jobs;
using Indico.Query;
using Indico.Types;

namespace Indico.Mutation
{
    /// <summary>
    /// Result of a Submission.
    /// </summary>
    public class SubmissionResult : IMutation<Job>
    {
        private readonly IndicoClient _client;

        /// <summary>
        /// Submission id.
        /// </summary>
        public int SubmissionId { get; set; }

        /// <summary>
        /// Submission expected status.
        /// </summary>
        public SubmissionStatus? ExpectedStatus { get; set; }

        /// <summary>
        /// SubmissionResult constructor.
        /// </summary>
        /// <param name="client">Client used to send API requests.</param>
        public SubmissionResult(IndicoClient client) => _client = client;

        /// <summary>
        /// Executes request and returns job.
        /// </summary>
        public async Task<Job> Exec(CancellationToken cancellationToken = default)
        {
            var getSubmission = new GetSubmission(_client)
            {
                Id = SubmissionId
            };
            var submission = await getSubmission.Exec(cancellationToken);

            while(!StatusCheck(submission.Status))
            {
                submission = await getSubmission.Exec(cancellationToken);
                await Task.Delay(1000);
            }

            if (!StatusCheck(submission.Status))
            {
                throw new RuntimeException($"Submission {submission.Id} does not meet status requirements");
            }

            var generateSubmissionResult = new GenerateSubmissionResult(_client)
            {
                SubmissionId = submission.Id
            };
            var job = await generateSubmissionResult.Exec();

            return job;
        }

        private bool StatusCheck(SubmissionStatus status)
        {
            if(ExpectedStatus != null)
            {
                return status.Equals(ExpectedStatus);
            }

            return !status.Equals(SubmissionStatus.PROCESSING);
        }
    }
}
