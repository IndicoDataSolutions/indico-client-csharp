using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IndicoV2.Submissions.Models;

namespace IndicoV2.V1Adapters.Submissions.Models
{
    public class V1SubmissionFilterAdapter
    {
        private readonly IFilter _filter;

        public Indico.Entity.SubmissionFilter FilterLegacy => ConvertToLegacyFilter(_filter);

        public V1SubmissionFilterAdapter(IFilter filter)
        {
            _filter = filter;
        }

        private Indico.Entity.SubmissionFilter ConvertToLegacyFilter(IFilter filter)
        {
            if (_filter is SubmissionFilter submissionFilter)
            {
                return new Indico.Entity.SubmissionFilter()
                {
                    InputFilename = submissionFilter.InputFilename,
                    Retrieved = submissionFilter.Retrieved,
                    //Status = Indico.Types.SubmissionStatus.COMPLETE //TODO
                };
            }
            else if (_filter is AndFilter andFilter)
            {
                return new Indico.Entity.SubmissionFilter()
                {
                    AND = andFilter.And.Select(a => ConvertToLegacyFilter(a)).ToList()
                };
            }
            else if (_filter is OrFilter orFilter)
            {
                return new Indico.Entity.SubmissionFilter()
                {
                    OR = orFilter.Or.Select(a => ConvertToLegacyFilter(a)).ToList()
                };
            }

            throw new NotSupportedException();
        }
    }
}
