using System;
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

        private int? _submissionId;

        /// <summary>
        /// Submission id.
        /// </summary>
        public int SubmissionId
        {
            get
            {
                if (!_submissionId.HasValue)
                {
                    throw new ArgumentNullException($"{nameof(SubmissionId)} has no value.");
                }

                return _submissionId.Value;
            }

            set => _submissionId = value;
        }

        /// <summary>
        /// Submission expected status.
        /// </summary>
        public SubmissionStatus? CheckStatus { get; set; }

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

            while (!StatusCheck(submission.Status))
            {
                submission = await getSubmission.Exec(cancellationToken);
                await Task.Delay(1000);
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
            if (CheckStatus != null)
            {
                return status.Equals(CheckStatus);
            }

            return !status.Equals(SubmissionStatus.PROCESSING);
        }
    }
}
