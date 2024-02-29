using System;
using System.Collections.Generic;
using System.Text;

using v2SubmissionFilter = IndicoV2.Submissions.Models.SubmissionFilter;
using ssSubmissionFilter = IndicoV2.StrawberryShake.SubmissionFilter;
using System.Linq;
using IndicoV2.StrawberryShake;

namespace IndicoV2.Submissions.Models
{
    public static class FilterConverter
    {

        public static ssSubmissionFilter ConvertToSs(this IFilter filter)
        {
            if (filter is v2SubmissionFilter submissionFilter)
            {
                return new ssSubmissionFilter()
                {
                    Filetype = submissionFilter.FileType.Select(x => (StrawberryShake.FileType)x).ToList(),
                    InputFilename = submissionFilter.InputFilename,
                    Retrieved = submissionFilter.Retrieved,
                    Status = (StrawberryShake.SubmissionStatus?)submissionFilter.Status,
                    Reviews = new ReviewFilter() {
                        Rejected = submissionFilter.Reviews.Rejected,
                        CreatedBy = submissionFilter.Reviews.CreatedBy,
                        ReviewType = (StrawberryShake.ReviewType) submissionFilter.Reviews.ReviewType
                    },
                    ReviewInProgress = submissionFilter.ReviewInProgress,
                    FilesDeleted = submissionFilter.FilesDeleted,
                    CreatedAt = new StrawberryShake.DateRangeFilter() {
                        From = submissionFilter.CreatedAt.From,
                        To = submissionFilter.CreatedAt.To,
                    },
                    UpdatedAt = new StrawberryShake.DateRangeFilter() {
                        From = submissionFilter.UpdatedAt.From,
                        To = submissionFilter.UpdatedAt.To
                    }
                };
            }
            else if (filter is AndFilter andfilter)
            {
                return new ssSubmissionFilter()
                {
                    AND = andfilter.And.Select(a => a.ConvertToSs()).ToList()
                };

            }
            else if (filter is OrFilter orFilter)
            {
                return new ssSubmissionFilter()
                {
                    OR = orFilter.Or.Select(a => a.ConvertToSs()).ToList()
                };
            }
            throw new NotSupportedException("Found a filter type we don't support. Check that all filters are the right type.");
        }
    }
}
