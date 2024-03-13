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
                // Note: you have to not reference or set a field at all for StrawberryShake Code Generator to decide to not serialize it
                var ssFilter = new ssSubmissionFilter();
                if (submissionFilter.FileType != null)
                {
                    ssFilter.Filetype = submissionFilter.FileType.Select(x => (FileType)Enum.Parse(typeof(FileType), x)).ToList();
                }
                if (!string.IsNullOrEmpty(submissionFilter.InputFilename))
                {
                    ssFilter.InputFilename = submissionFilter.InputFilename;
                }
                if (submissionFilter.Retrieved.HasValue)
                {
                    ssFilter.Retrieved = submissionFilter.Retrieved;
                }
                if (submissionFilter.Status.HasValue)
                {
                    ssFilter.Status = (StrawberryShake.SubmissionStatus?)submissionFilter.Status;
                }
                if (submissionFilter.Reviews != null)
                {
                    ssFilter.Reviews = new ReviewFilter()
                    {
                        Rejected = submissionFilter.Reviews.Rejected,
                        CreatedBy = submissionFilter.Reviews.CreatedBy,
                        ReviewType = (StrawberryShake.ReviewType)submissionFilter.Reviews.ReviewType
                    };
                }
                if (submissionFilter.ReviewInProgress.HasValue)
                {
                    ssFilter.ReviewInProgress = submissionFilter.ReviewInProgress;
                }
                if (submissionFilter.FilesDeleted.HasValue)
                {
                    ssFilter.FilesDeleted = submissionFilter.FilesDeleted;
                }
                if (submissionFilter.CreatedAt != null)
                {
                    ssFilter.CreatedAt = new StrawberryShake.DateRangeFilter()
                    {
                        From = submissionFilter.CreatedAt.From,
                        To = submissionFilter.CreatedAt.To,
                    };
                }
                if (submissionFilter.UpdatedAt != null)
                {
                    ssFilter.UpdatedAt = new StrawberryShake.DateRangeFilter()
                    {
                        From = submissionFilter.UpdatedAt.From,
                        To = submissionFilter.UpdatedAt.To
                    };
                }
                return ssFilter;
            }
            else if (filter is AndFilter andfilter)
            {
                Console.WriteLine("and");
                return new ssSubmissionFilter()
                {
                    AND = andfilter.And.Select(a => a.ConvertToSs()).ToList()
                };

            }
            else if (filter is OrFilter orFilter)
            {
                Console.WriteLine("or");
                return new ssSubmissionFilter()
                {
                    OR = orFilter.Or.Select(a => a.ConvertToSs()).ToList()
                };
            }
            throw new NotSupportedException("Found a filter type we don't support. Check that all filters are the right type.");
        }
    }
}
