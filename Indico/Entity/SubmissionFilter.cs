using Indico.Types;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Indico.Entity
{
    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    public class SubmissionFilter : Filter
    {
        public string InputFilename { get; set; }
        public SubmissionStatus? Status { get; set; }
        public bool? Retrieved { get; set; }
        public List<SubmissionFilter> OR { get; set; }
        public List<SubmissionFilter> AND { get; set; }

        public override dynamic ToAnonymousType()
        {
            dynamic anonymousType = new ExpandoObject();
            if (InputFilename != null)
            {
                anonymousType.inputFilename = InputFilename;
            }

            if (Status != null)
            {
                anonymousType.status = Status.ToString();
            }

            if (Retrieved != null)
            {
                anonymousType.retrieved = Retrieved;
            }

            if (OR != null)
            {
                anonymousType.ors = MergeFilters(OR);
            }

            if (AND != null)
            {
                anonymousType.ands = MergeFilters(AND);
            }

            return anonymousType;
        }
    }
}
