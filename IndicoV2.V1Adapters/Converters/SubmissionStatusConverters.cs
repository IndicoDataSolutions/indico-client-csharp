using System;

using v1SubmissionStatus = Indico.Types.SubmissionStatus;
using v2SubmissionStatus = IndicoV2.Submissions.Models.SubmissionStatus;

namespace IndicoV2.V1Adapters.Converters
{
    public static class SubmissionStatusConverters
    {
        public static v1SubmissionStatus? ConvertToLegacy(this v2SubmissionStatus? status)
        {
            if (!status.HasValue)
                return null;

            if (!Enum.TryParse(status.ToString(), out v1SubmissionStatus parsed))
            {
                throw new NotImplementedException();
            }

            return parsed;
        }

        public static v2SubmissionStatus ConvertFromLegacy(this v1SubmissionStatus legacyStatus)
        {
            if (!Enum.TryParse(legacyStatus.ToString(), out v2SubmissionStatus parsed))
            {
                throw new NotImplementedException();
            }

            return parsed;
        }
    }
}
