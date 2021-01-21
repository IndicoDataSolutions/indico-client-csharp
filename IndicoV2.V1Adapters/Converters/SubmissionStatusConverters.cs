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

            if (!Enum.TryParse(status.ToString(), out Indico.Types.SubmissionStatus parsed))
            {
                throw new NotImplementedException();
            }

            return parsed;
        }

        public static SubmissionStatus ConvertFromLegacy(this Indico.Types.SubmissionStatus legacyStatus)
        {
            if (!Enum.TryParse(legacyStatus.ToString(), out SubmissionStatus parsed))
            {
                throw new NotImplementedException();
            }

            return parsed;
        }
    }
}
