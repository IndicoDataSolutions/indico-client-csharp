using System;
using IndicoV2.Submissions.Models;

namespace IndicoV2.Extensions.SubmissionResult.Exceptions
{
    /// <summary>
    /// Thrown when trying to get submission result and because of wrong status result cannot be downloaded.
    /// </summary>
    public class WrongSubmissionStatusException : Exception
    {
        public WrongSubmissionStatusException() { }

        public WrongSubmissionStatusException(string message) : base(message) { }

        public WrongSubmissionStatusException(SubmissionStatus status) : base($"Cannot get the result because submission status is { status }.") { }
    }
}
