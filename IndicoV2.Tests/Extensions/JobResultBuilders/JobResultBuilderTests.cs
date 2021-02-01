using System;
using FluentAssertions;
using IndicoV2.Extensions.JobResultBuilders;
using IndicoV2.Extensions.JobResultBuilders.Submission.Exceptions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace IndicoV2.Tests.Extensions.JobResultBuilders
{
    public class JobResultBuilderTests
    {
        private JobResultBuilder _sut;

        [SetUp]
        public void SetUp() => _sut = new JobResultBuilder();

        [TestCase(@"{ ""url"": ""test"" }", "test")]
        public void GetSubmissionJobResult_ShouldReturnUrlResult(string json, string expectedUrl) => _sut.GetSubmissionJobResult(JObject.Parse(json)).Url.Should().Be(expectedUrl);

        [Test]
        public void GetSubmissionJobResult_Throws_WhenJObjectNull() => Assert.Throws<ArgumentNullException>(() => _sut.GetSubmissionJobResult(null));

        // TODO: investigate what are invalid cases
        [TestCase(@"{ ""url"": null }")]
        public void GetSubmissionJobResult_ShouldThrow_WhenUrlInvalid(string invalidUrlJson) =>
            Assert.Throws<InvalidUrlException>(() =>
                _sut.GetSubmissionJobResult(JObject.Parse(invalidUrlJson)));

        [TestCase(@"{ }")]
        public void GetSubmissionJobResult_Throws_WhenUnsupportedJson(string json) =>
            Assert.Throws<InvalidJobSubmissionResult>(() => _sut.GetSubmissionJobResult(JObject.Parse(json)));
    }
}
