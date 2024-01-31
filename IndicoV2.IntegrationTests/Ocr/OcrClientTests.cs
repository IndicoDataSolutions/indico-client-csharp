using System;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.Extensions.Jobs;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
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
        private JobAwaiter _jobAwaiter;
        private IOcrClient _ocrClient;

        [SetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder()
                .ForAutoReviewWorkflow()
                .Build();
            _dataHelper = container.Resolve<DataHelper>();
            _jobAwaiter = container.Resolve<JobAwaiter>();
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
            var jobResult = await _jobAwaiter.WaitReadyAsync<ExtractionJobResult>(jobId, TimeSpan.FromMilliseconds(300));
            Console.WriteLine(jobResult.Url);
            if (preset != DocumentExtractionPreset.OnDocument)
            {
                // Act
                var extractionResult = await _ocrClient.GetExtractionResultAsync(jobResult.Url, default);
                Console.WriteLine(extractionResult);
                // Assert
                extractionResult.Should().NotBeNullOrEmpty();
            }
            else
            {
                // Act
                var extractionResult = await _ocrClient.GetExtractionResultAsync<JArray>(jobResult.Url, default);

                // Assert
                extractionResult.Count.Should().Be(1);
            }
        }
    }
}
