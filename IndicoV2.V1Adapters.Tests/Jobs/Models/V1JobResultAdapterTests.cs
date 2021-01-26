using System;
using FluentAssertions;
using IndicoV2.V1Adapters.Jobs.Models;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace IndicoV2.V1Adapters.Tests.Jobs.Models
{
    public class V1JobResultAdapterTests
    {
        [TestCase("indico-file:///storage/submission/1/2/result_4e47aaa5-24da-4365-81df-09d1d7782c30.json")]
        public void Url_ShouldReturnUri(string url)
        {
            // Arrange
            var jObject = JObject.Parse($@"{{""url"": ""{url}""}}");
            var adapter = V1JobResultAdapter.FromJson(jObject);
            
            // Act, Assert
            adapter.Url.Should().BeOfType<Uri>();
            adapter.Url.ToString().Should().Be(url);
        }
    }
}
