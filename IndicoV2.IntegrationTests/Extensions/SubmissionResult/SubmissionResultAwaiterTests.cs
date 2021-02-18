using System;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.Extensions.SubmissionResult;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.Extensions.SubmissionResult
{
    public class SubmissionResultAwaiterTests
    {
        private SubmissionResultAwaiter _sut;
        private DataHelper _dataHelper;

        [SetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();

            _sut = container.Resolve<SubmissionResultAwaiter>();
            _dataHelper = container.Resolve<DataHelper>();
        }

        [Test]
        public async Task WaitReady_ShouldReturnJobResult()
        {
            // Arrange
            var submissionId = (await _dataHelper.Submissions().GetAnyAsync()).Id;

            // Act
            var jobResult = await _sut.WaitReady(submissionId, TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(5), default);

            // Assert
            jobResult.Should().NotBeNull();
            jobResult["results"]["document"].Should().NotBeNull();
        }
    }
}
