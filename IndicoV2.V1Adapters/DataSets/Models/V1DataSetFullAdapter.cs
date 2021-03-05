using IndicoV2.DataSets.Models;
using IndicoV2.V1Adapters.DataSets.Converters;
using Newtonsoft.Json;

namespace IndicoV2.V1Adapters.DataSets.Models
{
    internal class V1DataSetFullAdapter : V1DataSetAdapter, IDataSetFull
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public int NumModelGroups { get; internal set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public int RowCount { get; internal set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public string Status { get; internal set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include, ItemConverterType = typeof(ModelGroupConverter))]
        public IModelGroup[] ModelGroups { get; internal set; }
    }
}
