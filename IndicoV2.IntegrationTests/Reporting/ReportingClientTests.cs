using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.Jobs;
using IndicoV2.Reporting;
using IndicoV2.Storage;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.Reporting
{
    public class ReportingClientTests
    {
        private IUserReportingClient _userReportingClient;
        private IJobsClient _jobsClient;
        private IStorageClient _storage;

        [OneTimeSetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            _userReportingClient = container.Resolve<IUserReportingClient>();
            container.Resolve<IJobsClient>();
            _jobsClient = container.Resolve<IJobsClient>();
            _storage = container.Resolve<IStorageClient>();
        }

        [Test]
        public async Task SnapReportAsync_ShouldGenerateReport()
        {
            var jobId = (await _userReportingClient.CreateSnapshotReportAsync(default, null, default))
                .UserSnapshotReport.JobId;

            await AssertJobGeneratesNonEmptyReport(jobId);
        }

        [Test]
        public async Task ChangeReportAsync_ShouldGenerateReport()
        {
            var jobId = (await _userReportingClient.CreateChangelogReportAsync(null, null, null, default))
                .UserChangelogReport.JobId;

            await AssertJobGeneratesNonEmptyReport(jobId);
        }

        [Test]
        public async Task UserChangelog_ShouldReturnChangelog() =>
            (await _userReportingClient.UserChangelogQuery(default, default, default, default))
            .UserChangelog.Results
            .Should().NotBeEmpty();

        [Test]
        public async Task UserSummary_ShouldReturnSummary()
        {
            var summary = (await _userReportingClient.UserSummary(default, default));
            summary.UserSummary.Should().NotBeNull();
        }

        [Test]
        public async Task UserSnapshot_ShouldReturnSnapshot()
        {
            var snapshot = (await _userReportingClient.UserSnapshot(default, default, default));
            snapshot.UserSnapshot.Results.Should().NotBeEmpty();
        }

        private async Task AssertJobGeneratesNonEmptyReport(string jobId)
        {
            jobId.Should().NotBeNullOrWhiteSpace();

            var jobResult = JObject.Parse(await _jobsClient.GetResultAsync(jobId, default, default));
            var reportStream = await _storage.GetAsync(new Uri(jobResult.Value<string>("url")), default);
            using var reader = new StreamReader(reportStream);
            var report = await reader.ReadToEndAsync();
            report.Should().NotBeNullOrEmpty();
        }
    }
}
