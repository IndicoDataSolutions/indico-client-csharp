using System;
using IndicoV2.Jobs.Models;
using Newtonsoft.Json.Linq;

namespace IndicoV2.V1Adapters.Jobs.Models
{
    internal class V1JobResultAdapter : IJobResult
    {
        public Uri Url { get; set; }

        public static V1JobResultAdapter FromJson(JObject jObject) =>
            jObject.ToObject<V1JobResultAdapter>();
    }
}
