using IndicoV2.DataSets.Models;
using Newtonsoft.Json.Linq;

namespace IndicoV2.V1Adapters.DataSets.Models
{
    internal class V1DataSetAdapter : IDataSet
    {
        private readonly JToken _jToken;

        public V1DataSetAdapter(JToken jToken) => _jToken = jToken;

        public int Id => _jToken.Value<int>("id");
        public string Name => _jToken.Value<string>("name");
    }
}
