using System.Collections.Generic;
using Newtonsoft.Json;

namespace IndicoV2.V1Adapters.CommonModels.Predictions
{
    public class Prediction : IPrediction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)] 
        public string Label { get; internal set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)] 
        public string Text { get; internal set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)] 
        public int Start { get; internal set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)] 
        public int End { get; internal set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public IReadOnlyDictionary<string, double> Confidence { get; internal set; }
    }
}
