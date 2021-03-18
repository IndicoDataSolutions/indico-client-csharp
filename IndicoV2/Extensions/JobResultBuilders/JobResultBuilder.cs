using IndicoV2.Extensions.JobResultBuilders.Submission;
using IndicoV2.Extensions.JobResultBuilders.Submission.Exceptions;
using IndicoV2.Jobs.Models.Results;
using Newtonsoft.Json.Linq;

namespace IndicoV2.Extensions.JobResultBuilders
{
    internal class JobResultBuilder
    {
        public IUrlJobResult GetSubmissionJobResult(JObject result) => UrlJobResult.TryGet(result) ?? throw new InvalidJobSubmissionResult();
    }
}
