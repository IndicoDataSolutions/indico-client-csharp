using System;
using IndicoV2.Jobs.Models;

namespace IndicoV2.Jobs.Exceptions
{
    /// <summary>
    /// Thrown when job trying to get job which haven't finished yet, or have finished with an error.
    /// </summary>
    public class JobNotSuccessfulException : Exception
    {
        public JobNotSuccessfulException(JobStatus status) : base($"Job haven't finished or failed: {status}")
        { }
    }
}
