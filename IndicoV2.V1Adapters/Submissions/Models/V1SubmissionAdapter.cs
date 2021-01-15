using System;
using IndicoV2.Submissions.Models;

namespace IndicoV2.V1Adapters.Submissions.Models
{
    public class V1SubmissionAdapter : ISubmission
    {
        private readonly Indico.Entity.Submission _submissionLegacy;

        public V1SubmissionAdapter(Indico.Entity.Submission submissionLegacy)
        {
            _submissionLegacy = submissionLegacy;
        }

        public int Id => _submissionLegacy.Id;

        public SubmissionStatus Status
        {
            get {
                switch (_submissionLegacy.Status)
                {
                    case Indico.Types.SubmissionStatus.PROCESSING: return SubmissionStatus.PROCESSING;
                    case Indico.Types.SubmissionStatus.FAILED: return SubmissionStatus.FAILED;
                    case Indico.Types.SubmissionStatus.COMPLETE: return SubmissionStatus.COMPLETE;
                    case Indico.Types.SubmissionStatus.PENDING_ADMIN_REVIEW: return SubmissionStatus.PENDING_ADMIN_REVIEW;
                    case Indico.Types.SubmissionStatus.PENDING_REVIEW: return SubmissionStatus.PENDING_REVIEW;

                    default: throw new NotImplementedException();
                }
            }
        }
    }
}