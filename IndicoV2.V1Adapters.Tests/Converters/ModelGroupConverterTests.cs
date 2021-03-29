using FluentAssertions;
using IndicoV2.Models.Models;
using IndicoV2.V1Adapters.Converters;
using IndicoV2.V1Adapters.Models.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace IndicoV2.V1Adapters.Tests.Converters
{
    public class ModelGroupConverterTests
    {
        [Test]
        public void Deserialize_ShouldCreateV1ModelGroupBaseAdapter_ForInterface() =>
            JsonConvert.DeserializeObject<TestClass>(
                "{ Group: { Id: 1 } }")
                .Group.Id.Should().Be(1);

        private class TestClass
        {
            [JsonConverter(typeof(InterfaceConverter<IModelGroupBase, V1ModelGroupBaseAdapter>))]
            public IModelGroupBase Group { get; set; }
        }
    }
}
