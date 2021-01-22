using System;
using System.Linq;

using IndicoV2.Submissions.Models;

using v1SubmissionFilter = Indico.Entity.SubmissionFilter;
using v2SubmissionFilter = IndicoV2.Submissions.Models.SubmissionFilter;

namespace IndicoV2.V1Adapters.Converters
{
    public static class SubmissionFilterConverters
    {
        public static v1SubmissionFilter ConvertToLegacy(this IFilter filter)
        {
            if (filter is v2SubmissionFilter submissionFilter)
            {
                return new v1SubmissionFilter()
                {
                    InputFilename = submissionFilter.InputFilename,
                    Retrieved = submissionFilter.Retrieved,
                    Status = submissionFilter.Status.ConvertToLegacy()
                };
            }
            else if (filter is AndFilter andFilter)
            {
                return new v1SubmissionFilter()
                {
                    AND = andFilter.And.Select(a => a.ConvertToLegacy()).ToList()
                };
            }
            else if (filter is OrFilter orFilter)
            {
                return new v1SubmissionFilter()
                {
                    OR = orFilter.Or.Select(a => a.ConvertToLegacy()).ToList()
                };
            }

            throw new NotSupportedException();
        }
    }
}
