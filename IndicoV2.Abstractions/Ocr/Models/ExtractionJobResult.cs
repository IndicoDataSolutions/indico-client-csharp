using System;
using IndicoV2.Jobs.Models.Results;
using Newtonsoft.Json;

namespace IndicoV2.Ocr.Models
{
    public class ExtractionJobResult : IUrlJobResult
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public Uri Url { get; internal set; }
    }
}
