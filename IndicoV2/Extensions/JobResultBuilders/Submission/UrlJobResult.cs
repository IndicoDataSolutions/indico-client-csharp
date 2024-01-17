using System;
using IndicoV2.Extensions.JobResultBuilders.Submission.Exceptions;
using IndicoV2.Jobs.Models.Results;
using Newtonsoft.Json.Linq;

namespace IndicoV2.Extensions.JobResultBuilders.Submission
{
    internal class UrlJobResult : IUrlJobResult
    {
        private const string _urlJsonPropertyName = "url";
        public Uri Url { get; set; }

        public static UrlJobResult TryGet(JObject resultJson)
        {
            if (resultJson == null)
            {
                throw new ArgumentNullException(nameof(resultJson));
            }

            if (resultJson[_urlJsonPropertyName] == null)
            {
                return null;
            }

            var urlResult = resultJson.ToObject<UrlJobResult>();

            if (urlResult.Url == null)
            {
                throw new InvalidUrlException();
            }

            return urlResult;
        }
    }
}