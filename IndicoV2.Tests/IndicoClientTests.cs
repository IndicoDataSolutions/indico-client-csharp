using System;
using FluentAssertions;
using NUnit.Framework;

namespace IndicoV2.Tests
{
    public class IndicoClientTests
    {
        [Test]
        public void IndicoClient_ShouldPointToProd_WhenNoUriPassed() =>
            new IndicoClient("testToken").BaseUri.Should().Be("https://try.indico.io");

        [Test]
        public void IndicoClient_ShouldThrowWhenNullToken() =>
            this.Invoking(_ => new IndicoClient(null))
                .Should().Throw<ArgumentNullException>()
                .WithMessage("*apiToken*");

        [TestCase("https://dev-ci.us-east-2.indico-dev.indico.io")]
        public void IndicoClient_ShouldPointToUri_WhenPassedAsParam(string uri) =>
            new IndicoClient("testToken", new Uri(uri)).BaseUri.Should().Be(uri);

        [Test]
        public void IndicoClient_ShouldThrow_WhenNullUri() =>
            this.Invoking(_ => new IndicoClient("testToken", null))
                .Should().Throw<ArgumentNullException>()
                .WithMessage("*baseUri*");
    }
}
