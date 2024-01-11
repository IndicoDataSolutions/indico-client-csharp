using System;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.Jobs;
using IndicoV2.Ocr;
using IndicoV2.Ocr.Models;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.Ocr
{
    public class OcrClientTests
    {
        private DataHelper _dataHelper;
        private IJobsClient _jobsClient;
        private IOcrClient _ocrClient;

        [SetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder()
                .ForAutoReviewWorkflow()
                .Build();
            _dataHelper = container.Resolve<DataHelper>();
            _jobsClient = container.Resolve<IJobsClient>();
            _ocrClient = container.Resolve<IOcrClient>();
        }

        [Theory]
        public async Task ExtractDocument_ShouldReturnJobNumber(DocumentExtractionPreset preset)
            => (await _ocrClient.ExtractDocumentAsync(_dataHelper.Files().GetSampleFilePath(), preset, default))
                .Should().NotBeEmpty();

        [Theory]
        public async Task GetExtractionResult_ShouldReturnDocument(DocumentExtractionPreset preset)
        {
            // Arrange
            var jobId = await _ocrClient.ExtractDocumentAsync(_dataHelper.Files().GetSampleFilePath(), preset);
            var jobResult = await _jobsClient.GetResultAsync(jobId, default, default);
            var jobResultUri = new Uri(JToken.Parse(jobResult).Value<string>("url"));

            if (preset != DocumentExtractionPreset.OnDocument)
            {
                // Act
                var extractionResult = await _ocrClient.GetExtractionResultAsync(jobResultUri, default);
                // Assert
                extractionResult.Should().NotBeNullOrEmpty();
            }
            else
            {
                // Act
                var extractionResult = await _ocrClient.GetExtractionResultAsync<JArray>(jobResultUri, default);

                // Assert
                extractionResult.Count.Should().Be(1);
            }
        }
    }
}
