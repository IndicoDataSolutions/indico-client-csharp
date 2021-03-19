using System;
using IndicoV2.Models.Models;
using IndicoV2.V1Adapters.Models.Models;
using Newtonsoft.Json;

namespace IndicoV2.V1Adapters.Converters
{
    internal class ModelGroupConverter : JsonConverter<IModelGroupBase>
    {
        public override void WriteJson(JsonWriter writer, IModelGroupBase value, JsonSerializer serializer) =>
            serializer.Serialize(writer, value);

        public override IModelGroupBase ReadJson(JsonReader reader, Type objectType, IModelGroupBase existingValue,
            bool hasExistingValue,
            JsonSerializer serializer) => serializer.Deserialize<V1ModelGroupBaseAdapter>(reader);
    }
}
