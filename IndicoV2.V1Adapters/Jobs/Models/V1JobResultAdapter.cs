using System;
using IndicoV2.Jobs.Models;
using Newtonsoft.Json.Linq;

namespace IndicoV2.V1Adapters.Jobs.Models
{
    public class V1JobResultAdapter : IJobResult
    {
        private readonly JObject _jobResult;

        public V1JobResultAdapter(JObject jobResult) => _jobResult = jobResult;

        public Uri Url => new Uri(_jobResult.Value<string>("url"));
    }
}