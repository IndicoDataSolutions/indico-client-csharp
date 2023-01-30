using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.DataSets;
using IndicoV2.Extensions.Jobs;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.Models;
using NUnit.Framework;
using Unity;
using ModelStatus = Indico.Types.ModelStatus;

namespace IndicoV2.IntegrationTests.Models
{
    public class ModelClientTests
    {
        private IModelClient _modelClient;
        private int _modelGroupId;
        private IJobAwaiter _jobAwaiter;
        private int _modelId;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();

            _modelClient = container.Resolve<IModelClient>();
            var dataSets = await container.Resolve<IDataSetClient>().ListFullAsync(1);
            _modelGroupId = dataSets.First().ModelGroups.First().Id;
            _modelId = (await _modelClient.GetGroup(_modelGroupId, default)).Id;
            _jobAwaiter = container.Resolve<IJobAwaiter>();
        }

        [Test]
        public async Task GetGroup_ShouldReturnModelGroup()
        {
            var modelGroup = await _modelClient.GetGroup(_modelGroupId, default);

            modelGroup.Id.Should().Be(_modelGroupId);
            modelGroup.Name.Should().NotBeEmpty();
            //modelGroup.Status.Should().NotBe(ModelStatus.CREATING);

            var selectedModel = modelGroup.SelectedModel;
            selectedModel.Should().NotBeNull();

            selectedModel.Id.Should().BeGreaterThan(0);
            selectedModel.Status.Should().NotBeEmpty();
            selectedModel.TrainingProgressPercents.Should().BeGreaterOrEqualTo(0);
        }

        [Test]
        public async Task LoadModel_ShouldReturnStatus() =>
            (await _modelClient.LoadModel(_modelId, default)).Should().Be("ready");

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
    }
}
