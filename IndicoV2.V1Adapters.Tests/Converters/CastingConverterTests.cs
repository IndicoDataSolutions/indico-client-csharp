using FluentAssertions;
using IndicoV2.V1Adapters.Converters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace IndicoV2.V1Adapters.Tests.Converters
{
    public class CastingConverterTests
    {
        [TestCase(1)]
        public void CastingConverter_ShouldUseImplicitCastOperator(int value) => 
            JsonConvert.DeserializeObject<TestClass>(value.ToString()).ValueProperty.Should().Be(value);

        [JsonConverter(typeof(CastingConverter<int>))]
        private class TestClass
        {
            public int ValueProperty { get; private set; }

            public static implicit operator TestClass(int value) => new TestClass {ValueProperty = value};
        }
    }
}
