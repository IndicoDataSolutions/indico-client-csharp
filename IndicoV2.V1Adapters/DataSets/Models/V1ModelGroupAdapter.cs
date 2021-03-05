using IndicoV2.DataSets.Models;
using Newtonsoft.Json;

namespace IndicoV2.V1Adapters.DataSets.Models
{
    internal class V1ModelGroupAdapter : IModelGroup
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public int Id { get; internal set; }
    }
}
