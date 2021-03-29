using System;

namespace IndicoV2.Extensions.JobResultBuilders.Submission.Exceptions
{
    public class InvalidJobSubmissionResult : NotSupportedException
    {
        public InvalidJobSubmissionResult() { }

        public InvalidJobSubmissionResult(string message) : base(message) { }
    }
}
