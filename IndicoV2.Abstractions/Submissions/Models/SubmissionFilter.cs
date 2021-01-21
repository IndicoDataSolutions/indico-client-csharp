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
    }
}
