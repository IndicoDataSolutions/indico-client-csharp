using System;
using IndicoV2.DataSets.Models;
using IndicoV2.V1Adapters.DataSets.Models;
using Newtonsoft.Json;

namespace IndicoV2.V1Adapters.DataSets.Converters
{
    internal class ModelGroupConverter : JsonConverter<IModelGroup>
    {
        public override void WriteJson(JsonWriter writer, IModelGroup value, JsonSerializer serializer) =>
            serializer.Serialize(writer, value);

        public override IModelGroup ReadJson(JsonReader reader, Type objectType, IModelGroup existingValue,
            bool hasExistingValue,
            JsonSerializer serializer) => serializer.Deserialize<V1ModelGroupAdapter>(reader);
    }
}
