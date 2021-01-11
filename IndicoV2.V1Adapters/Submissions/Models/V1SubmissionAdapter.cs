using System;
using IndicoV2.Abstractions.Submissions.Models;

namespace IndicoV2.V1Adapters.Submissions.Models
{
    public class V1SubmissionAdapter : ISubmission
    {
        private readonly Indico.Entity.Submission _submissionLegacy;

        public V1SubmissionAdapter(Indico.Entity.Submission submissionLegacy)
        {
            _submissionLegacy = submissionLegacy;
        }

        public SubmissionStatus Status
        {
            get {
                switch (_submissionLegacy.Status)
                {
                    case Indico.Types.SubmissionStatus.PROCESSING: return SubmissionStatus.PROCESSING;
                    case Indico.Types.SubmissionStatus.FAILED: return SubmissionStatus.FAILED;
                    default: throw new NotImplementedException();
                }
            }
        }
    }
}