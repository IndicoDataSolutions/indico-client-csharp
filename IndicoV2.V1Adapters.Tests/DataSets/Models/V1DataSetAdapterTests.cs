using FluentAssertions;
using IndicoV2.V1Adapters.DataSets.Models;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace IndicoV2.V1Adapters.Tests.DataSets.Models
{
    public class V1DataSetAdapterTests
    {
        [Test]
        public void Id_ReturnsInnerId() =>
            new V1DataSetAdapter(JToken.Parse(@"{""id"": 1}"))
            .Id.Should().Be(1);

        [Test]
        public void Name_ReturnsInnerName() =>
            new V1DataSetAdapter(JToken.Parse(@"{""name"": ""test""}"))
                .Name.Should().Be("test");
    }
}
