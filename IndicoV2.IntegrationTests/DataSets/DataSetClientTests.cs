using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.DataSets;
using IndicoV2.IntegrationTests.Utils;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.DataSets
{
    public class DataSetClientTests
    {
        private IDataSetClient _dataSetClient;

        [SetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            _dataSetClient = container.Resolve<IDataSetClient>();
        }

        [Test]
        public async Task ListAsync_ShouldReturnDataSets()
        {
            var dataSets = (await _dataSetClient.ListAsync()).ToArray();

            dataSets.Length.Should().BeGreaterThan(0);
            var ds = dataSets.First();
            ds.Id.Should().BeGreaterThan(0);
            ds.Name.Should().NotBeNullOrEmpty();
        }

        [TestCase(null)]
        [TestCase(1)]
        public async Task ListFullAsync_ShouldReturnDataSetsFull(int? limit)
        {
            var dataSets = (await _dataSetClient.ListFullAsync(limit)).ToArray();

            if (limit.HasValue)
            {
                dataSets.Length.Should().Be(limit);
            }
            else
            {
                dataSets.Length.Should().BeGreaterThan(0);
            }

            var ds = dataSets.First();
            ds.Id.Should().BeGreaterThan(0);
            ds.Name.Should().NotBeEmpty();
            ds.Status.Should().NotBeEmpty();
            ds.ModelGroups.Should().NotBeEmpty();
            ds.NumModelGroups.Should().BeGreaterThan(0);
            ds.RowCount.Should().BeGreaterThan(0);
        }
    }
}
