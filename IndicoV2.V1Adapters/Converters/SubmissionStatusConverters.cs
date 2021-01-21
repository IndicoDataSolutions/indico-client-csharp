using System;

using IndicoV2.Submissions.Models;

namespace IndicoV2.V1Adapters.Converters
{
    public static class SubmissionStatusConverters
    {
        public static Indico.Types.SubmissionStatus? ConvertToLegacy(this SubmissionStatus? status)
        {
            if (!status.HasValue)
                return null;

            if (!Enum.IsDefined(typeof(Indico.Types.SubmissionStatus), status))
                throw new NotImplementedException();

            return (Indico.Types.SubmissionStatus)status;
        }

        public static SubmissionStatus? ConvertFromLegacy(this Indico.Types.SubmissionStatus? legacyStatus)
        {
            if (!legacyStatus.HasValue)
                return null;

            if (!Enum.IsDefined(typeof(SubmissionStatus), legacyStatus))
                throw new NotImplementedException();

            return (SubmissionStatus)legacyStatus;
        }
    }
}
