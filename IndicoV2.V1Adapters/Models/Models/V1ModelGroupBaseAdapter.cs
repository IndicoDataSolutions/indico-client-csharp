using IndicoV2.Models.Models;
using Newtonsoft.Json;

namespace IndicoV2.V1Adapters.Models.Models
{
    internal class V1ModelGroupBaseAdapter : IModelGroupBase
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public int Id { get; internal set; }
    }
}
