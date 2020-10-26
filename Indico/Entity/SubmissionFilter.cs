using Indico.Types;
using System.Collections.Generic;

namespace Indico.Entity
{
    public class SubmissionFilter : Filter
    {
        public string InputFilename { get; set; }
        public SubmissionStatus Status { get; set; }
        public List<SubmissionFilter> OR { get; set; }
        public List<SubmissionFilter> AND { get; set; }

        public override dynamic ToAnonymousType()
        {
            return new
            {
                inputFilename = InputFilename,
                status = Status.ToString(),
                ors = MergeFilters(OR),
                ands = MergeFilters(AND)
            };
        }
    }
}
