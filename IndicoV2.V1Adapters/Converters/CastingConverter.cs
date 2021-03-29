using System;
using Newtonsoft.Json;

namespace IndicoV2.V1Adapters.Converters
{
    public class CastingConverter<TCastFrom> : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            serializer.Serialize(writer, value);

        public override object ReadJson(JsonReader reader, Type targetType, object existingValue, JsonSerializer serializer) 
            => Cast(serializer.Deserialize<TCastFrom>(reader), targetType);

        public override bool CanConvert(Type objectType) => throw new NotSupportedException("Only explicit usage is being allowed (class requiring this converter needs to be marked with attribute JsonConverter(typeof(CastingConverter<T>)) ).");

        private object Cast(TCastFrom sourceValue, Type targetTyped)
        {
            var castOperator = targetTyped.GetMethod("op_Implicit", new[] {typeof(TCastFrom)});

            if (castOperator == null)
            {
                throw new InvalidOperationException($"Cannot cast form ${typeof(TCastFrom).Name}, operator not found.");
            }

            return castOperator.Invoke(null, new object[] {sourceValue});
        }

    }
}
