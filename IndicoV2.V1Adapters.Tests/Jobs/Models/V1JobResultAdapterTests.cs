using System;
using FluentAssertions;
using IndicoV2.V1Adapters.Jobs.Models;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace IndicoV2.V1Adapters.Tests.Jobs.Models
{
    public class V1JobResultAdapterTests
    {
        [TestCase("https://test.indicoio.io/asdf")]
        public void Url_ShouldReturnUri(string url)
        {
            // Arrange
            var adapter = new V1JobResultAdapter(JObject.Parse($@"{{""url"": ""{url}""}}"));
            
            // Act, Assert
            adapter.Url.Should().BeOfType<Uri>();
            adapter.Url.ToString().Should().Be(url);
        }
    }
}
