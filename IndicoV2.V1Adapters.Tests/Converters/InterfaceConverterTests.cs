using FluentAssertions;
using IndicoV2.V1Adapters.Converters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace IndicoV2.V1Adapters.Tests.Converters
{
    public class InterfaceConverterTests
    {
        [Test]
        public void InterfaceConverter_ShouldConvertToInterface()
            => JsonConvert.DeserializeObject<Test>("{ 'Inner': {} }")
                .Inner.Should().NotBeNull()
                .And.BeOfType<TestInner>();

        [Test]
        public void InterfaceConverter_ShouldSerializeFromInterface() =>
            JsonConvert.SerializeObject(new Test {Inner = new TestInner()}).Should().Be("{\"Inner\":{}}");

        private interface ITestInner
        { }
        private class TestInner : ITestInner
        { }

        private class Test
        {
            [JsonConverter(typeof(InterfaceConverter<ITestInner, TestInner>))]
            public ITestInner Inner { get; set; }
        }

    }
}
