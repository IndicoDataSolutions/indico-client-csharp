using IndicoV2.DataSets.Models;
using Newtonsoft.Json;

namespace IndicoV2.V1Adapters.DataSets.Models
{
    internal class V1DataSetAdapter : IDataSet
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public int Id { get; internal set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public string Name { get; internal set; }
    }
}
