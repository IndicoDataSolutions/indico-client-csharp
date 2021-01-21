﻿using System;
using System.Linq;

using IndicoV2.Submissions.Models;

namespace IndicoV2.V1Adapters.Converters
{
    public static class SubmissionFilterConverters
    {
        public static Indico.Entity.SubmissionFilter ConvertToLegacy(this IFilter filter)
        {
            if (filter is SubmissionFilter submissionFilter)
            {
                return new Indico.Entity.SubmissionFilter()
                {
                    InputFilename = submissionFilter.InputFilename,
                    Retrieved = submissionFilter.Retrieved,
                    Status = submissionFilter.Status.ConvertToLegacy()
                };
            }
            else if (filter is AndFilter andFilter)
            {
                return new Indico.Entity.SubmissionFilter()
                {
                    AND = andFilter.And.Select(a => a.ConvertToLegacy()).ToList()
                };
            }
            else if (filter is OrFilter orFilter)
            {
                return new Indico.Entity.SubmissionFilter()
                {
                    OR = orFilter.Or.Select(a => a.ConvertToLegacy()).ToList()
                };
            }

            throw new NotSupportedException();
        }
    }
}