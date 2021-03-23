using System;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.Extensions.Jobs;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.Ocr;
using IndicoV2.Ocr.Models;
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

        [TestCase("standard")]
        public async Task ExtractDocument_ShouldReturnJobNumber(string configType)
            => (await _ocrClient.ExtractDocumentAsync(_dataHelper.Files().GetSampleFilePath(), configType, default))
                .Should().NotBeEmpty();

        [Test]
        public async Task GetExtractionResult_ShouldReturnDocument()
        {
            // Arrange
            var jobId = await _ocrClient.ExtractDocumentAsync(_dataHelper.Files().GetSampleFilePath());
            var jobResult = await _jobAwaiter.WaitReadyAsync<ExtractionJobResult>(jobId, TimeSpan.FromMilliseconds(300));
            
            // Act
            var extractionResult = await _ocrClient.GetExtractionResultAsync(jobResult.Url);

            // Assert
            extractionResult.Should().NotBeNullOrEmpty();
        }
    }
}
