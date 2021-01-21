using System;

using IndicoV2.Submissions.Models;

namespace IndicoV2.V1Adapters.Converters
{
    public static class SubmissionStatusConverters
    {
        public static Indico.Types.SubmissionStatus? ConvertToLegacy(this SubmissionStatus? status)
        {
            switch (status)
            {
                case SubmissionStatus.PROCESSING:
                    return Indico.Types.SubmissionStatus.PROCESSING;
                case SubmissionStatus.FAILED:
                    return Indico.Types.SubmissionStatus.FAILED;
                case SubmissionStatus.COMPLETE:
                    return Indico.Types.SubmissionStatus.COMPLETE;
                case SubmissionStatus.PENDING_ADMIN_REVIEW:
                    return Indico.Types.SubmissionStatus.PENDING_ADMIN_REVIEW;
                case SubmissionStatus.PENDING_REVIEW:
                    return Indico.Types.SubmissionStatus.PENDING_REVIEW;

                default:
                    throw new NotImplementedException();
            }
        }

        public static SubmissionStatus? ConvertFromLegacy(this Indico.Types.SubmissionStatus? legacyStatus)
        {
            switch (legacyStatus)
            {
                case Indico.Types.SubmissionStatus.PROCESSING:
                    return SubmissionStatus.PROCESSING;
                case Indico.Types.SubmissionStatus.FAILED:
                    return SubmissionStatus.FAILED;
                case Indico.Types.SubmissionStatus.COMPLETE:
                    return SubmissionStatus.COMPLETE;
                case Indico.Types.SubmissionStatus.PENDING_ADMIN_REVIEW:
                    return SubmissionStatus.PENDING_ADMIN_REVIEW;
                case Indico.Types.SubmissionStatus.PENDING_REVIEW:
                    return SubmissionStatus.PENDING_REVIEW;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
