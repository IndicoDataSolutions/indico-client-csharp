using System;
using IndicoV2.Jobs.Models;

namespace IndicoV2.Jobs.Exceptions
{
    /// <summary>
    /// Thrown when trying to get a job which hasn't finished yet, or which has finished with an error.
    /// </summary>
    public class JobNotSuccessfulException : Exception
    {
        public JobNotSuccessfulException(JobStatus status, string failReason) : base($"Invalid job status ({status}): {failReason}")
        { }
    }
}
