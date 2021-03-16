using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Indico.Types;
using IndicoV2.DataSets;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.Models;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.Models
{
    public class ModelClientTests
    {
        private IModelClient _modelClient;
        private int _modelGroupId;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            var container= new IndicoTestContainerBuilder().Build();

            _modelClient = container.Resolve<IModelClient>();
            var dataSets = await container.Resolve<IDataSetClient>().ListFullAsync();
            _modelGroupId = dataSets.First().ModelGroups.First().Id;
        }

        [Test]
        public async Task GetGroup_ShouldReturnModelGroup()
        {
            var modelGroup = await _modelClient.GetGroup(_modelGroupId, default);
            
            modelGroup.Id.Should().Be(_modelGroupId);
            modelGroup.Name.Should().NotBeEmpty();
            modelGroup.Status.Should().NotBe(ModelStatus.CREATING);

            var selectedModel = modelGroup.SelectedModel;
            selectedModel.Should().NotBeNull();

            selectedModel.Id.Should().BeGreaterThan(0);
            selectedModel.Status.Should().NotBeEmpty();
            selectedModel.TrainingProgressPercents.Should().BeGreaterOrEqualTo(0);
        }
    }
}
