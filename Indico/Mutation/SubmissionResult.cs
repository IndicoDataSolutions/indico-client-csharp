using System.Threading;
using System.Threading.Tasks;
using Indico.Entity;
using Indico.Exception;
using Indico.Jobs;
using Indico.Query;
using Indico.Types;

namespace Indico.Mutation
{
    public class SubmissionResult : Mutation<Job>
    {
        IndicoClient _client;
        public int SubmissionId { get; set; }
        public SubmissionStatus? CheckStatus { get; set; }

        public SubmissionResult(IndicoClient client) => this._client = client;

        async public Task<Job> Exec()
        {
            GetSubmission getSubmission = new GetSubmission(this._client)
            {
                Id = this.SubmissionId
            };
            Submission submission = await getSubmission.Exec();
            while(!StatusCheck(submission.Status))
            {
                submission = await getSubmission.Exec();
                Thread.Sleep(1000);
            }

            if (!StatusCheck(submission.Status))
            {
                throw new RuntimeException($"Submission {submission.Id} does not meet status requirements");
            }

            GenerateSubmissionResult generateSubmissionResult = new GenerateSubmissionResult(this._client)
            {
                SubmissionId = submission.Id
            };
            Job job = await generateSubmissionResult.Exec();
            return job;
        }

        bool StatusCheck(SubmissionStatus status)
        {
            if(this.CheckStatus != null)
            {
                return status.Equals(this.CheckStatus);
            }
            return !status.Equals(SubmissionStatus.PROCESSING);
        }
    }
}
