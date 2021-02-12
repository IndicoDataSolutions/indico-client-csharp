using IndicoV2;
using NUnit.Framework;
using FluentAssertions;

namespace IndicoV2.Tests
{
    [TestFixture()]
    public partial class IndicoClientExtensionsTests
    {
        private IndicoClient Client => new IndicoClient("test");

        [Test()]
        public void Reviews_ShouldCreateInstance() => Client.Reviews().Should().NotBeNull();

        [Test()]
        public void GetSubmissionResultAwaiter_ShouldCreateInstance() =>
            Client.GetSubmissionResultAwaiter().Should().NotBeNull();

        [Test()]
        public void JobAwaiter_ShouldCreateInstance() => Client.JobAwaiter().Should().NotBeNull();

        [Test()]
        public void DataSets_ShouldCreateInstance() => Client.DataSets().Should().NotBeNull();

        [Test()]
        public void Workflows_ShouldCreateInstance() => Client.Workflows().Should().NotBeNull();
    }
}