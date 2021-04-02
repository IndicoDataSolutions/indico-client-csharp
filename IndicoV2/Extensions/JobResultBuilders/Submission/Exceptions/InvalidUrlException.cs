using System;

namespace IndicoV2.Extensions.JobResultBuilders.Submission.Exceptions
{
    public class InvalidUrlException : NotSupportedException
    {
        public InvalidUrlException() : base($"Cannot create {typeof(UrlJobResult)} from json: url null") { }

        public InvalidUrlException(string message) : base(message) { }
    }
}
