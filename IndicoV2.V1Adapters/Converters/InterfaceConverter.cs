using System;
using Newtonsoft.Json;

namespace IndicoV2.V1Adapters.Converters
{
    internal class InterfaceConverter<TInterface, TImplementation> : JsonConverter<TInterface>
        where TImplementation : TInterface
    {
        public override void WriteJson(JsonWriter writer, TInterface value, JsonSerializer serializer) =>
            serializer.Serialize(writer, value);

        public override TInterface ReadJson(JsonReader reader, Type objectType, TInterface existingValue, bool hasExistingValue, JsonSerializer serializer)
            => serializer.Deserialize<TImplementation>(reader);
    }
}
