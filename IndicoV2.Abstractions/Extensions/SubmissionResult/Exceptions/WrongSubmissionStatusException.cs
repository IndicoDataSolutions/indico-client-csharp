using System;

namespace IndicoV2.Extensions.SubmissionResult.Exceptions
{
    public class WrongSubmissionStatusException : Exception
    {
        public WrongSubmissionStatusException()
        {
        }

        public WrongSubmissionStatusException(string message) : base(message)
        {
        }

        public WrongSubmissionStatusException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
