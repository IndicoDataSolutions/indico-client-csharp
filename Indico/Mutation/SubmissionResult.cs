using System;
using System.Threading;
using System.Threading.Tasks;
using Indico.Exception;
using Indico.Jobs;
using Indico.Query;
using Indico.Types;

namespace Indico.Mutation
{
    public class SubmissionResult : IMutation<Job>
    {
        private readonly IndicoClient _client;

        private int? _submissionId;

        public int SubmissionId
        {
            get
            {
                if (!_submissionId.HasValue)
                {
                    throw new ArgumentNullException();
                }

                return _submissionId.Value;
            }

            set => _submissionId = value;
        }

        public SubmissionStatus? CheckStatus { get; set; }

        public SubmissionResult(IndicoClient client) => _client = client;

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
            if (CheckStatus != null)
            {
                return status.Equals(CheckStatus);
            }
            return !status.Equals(SubmissionStatus.PROCESSING);
        }
    }
}
