using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace IndicoV2.Submissions.Models
{
    public class SubmissionFilter : IFilter
    {
        public string InputFilename { get; set; }
        public SubmissionStatus? Status { get; set; }
        public bool? Retrieved { get; set; }

        public dynamic ToAnonymousType()
        {
            dynamic anonymousType = new ExpandoObject();

            if (InputFilename != null)
                anonymousType.inputFilename = InputFilename;
            if (Status != null)
                anonymousType.status = Status.ToString();
            if (Retrieved != null)
                anonymousType.retrieved = Retrieved;

            return anonymousType;
        }
    }
}
