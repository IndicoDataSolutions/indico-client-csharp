using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Indico.Types;
using IndicoV2.DataSets;
using IndicoV2.Extensions.Jobs;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.Configs;
using IndicoV2.Models;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.Models
{
    public class ModelClientTests
    {
        private IModelClient _modelClient;
        private int _modelGroupId;
        private IJobAwaiter _jobAwaiter;
        private int _modelId;
        private IndicoConfigs _indicoConfigs;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            var containerBuilder = new IndicoTestContainerBuilder();
            var container = containerBuilder.Build();
            _modelClient = container.Resolve<IModelClient>();
            _indicoConfigs = new IndicoConfigs();
            var dataSets = await container.Resolve<IDataSetClient>().ListFullAsync(1);
            _indicoConfigs = new IndicoConfigs();
            var _rawModelGroupId = _indicoConfigs.ModelGroupId;
            if (_rawModelGroupId == 0)
            {
                _modelGroupId = dataSets.First().ModelGroups.First().Id;
            }
            else
            {
                _modelGroupId = _rawModelGroupId;
            }
            _modelId = (await _modelClient.GetGroup(_modelGroupId, default)).Id;
            _jobAwaiter = container.Resolve<IJobAwaiter>();
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
            selectedModel.Status.Should().NotBeNull();
            selectedModel.TrainingProgressPercents.Should().BeGreaterOrEqualTo(0);
        }


        [TestCase("Invoice Date: 2012-01-02")]
        [TestCase("Invoice Date: 2012-02-03 Invoice Number: 123Test")]
        public async Task Predict_ShouldReturnPrediction(params string[] data)
        {
            // Arrange
            var modelGroup = await _modelClient.GetGroup(_modelGroupId, default);

            // Act
            var jobId = await _modelClient.Predict(modelGroup.SelectedModel.Id, data.ToList(), default);
            var predictionResults =
                await _jobAwaiter.WaitPredictionReadyAsync(jobId, TimeSpan.FromMilliseconds(300), default);

            // Assert
            predictionResults.Count.Should().Be(data.Length);

            var firstPrediction = predictionResults.First().First();
            firstPrediction.Label.Should().NotBeNullOrEmpty();
            firstPrediction.Text.Should().NotBeNullOrEmpty();

            firstPrediction.Start.Should().BeGreaterThan(0);
            firstPrediction.End.Should().BeGreaterThan(0);

            firstPrediction.Confidence.Should().NotBeEmpty();
            firstPrediction.Confidence.First().Value.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task TrainingModelWithProgress_ShouldReturnResult()
        {
            var result = await _modelClient.TrainingModelWithProgress(_modelGroupId, default);
            result.Should().NotBeNullOrEmpty();
            var firstResult = result.First();
            firstResult.Id.Should().BeGreaterThan(0);
            firstResult.Status.Should().NotBeNull();
            firstResult.TrainingProgressPercents.Should().BeGreaterThan(0);
        }
    }
}
