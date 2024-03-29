using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.Submissions;
using IndicoV2.Submissions.Models;
using IndicoV2.Workflows.Models;
using IndicoV2.IntegrationTests.Utils.Configs;
using NUnit.Framework;
using Unity;
using System;

namespace IndicoV2.IntegrationTests.Submissions
{
    public class SubmissionClientTests
    {
        private DataHelper _dataHelper;
        private ISubmissionsClient _submissionsClient;
        private int _workflowId;
        private IndicoConfigs _indicoConfigs;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            _submissionsClient = (SubmissionsClient)container.Resolve<ISubmissionsClient>();
            _dataHelper = container.Resolve<DataHelper>();
            _indicoConfigs = new IndicoConfigs();
            var _rawWorkflowId = _indicoConfigs.WorkflowId;
            if (_rawWorkflowId == 0)
            {
                var _workflow = await _dataHelper.Workflows().GetAnyWorkflow();
                _workflowId = _workflow.Id;
            }
            else
            {
                _workflowId = _rawWorkflowId;
            }
        }

        [Test]
        public async Task CreateAsync_ShouldCreateSubmission_FromStream()
        {
            // Arrange
            await using var fileStream = _dataHelper.Files().GetSampleFileStream();

            // Act

            var submissionIds = await _submissionsClient.CreateAsync(_workflowId, new List<(string f, Stream c)> { ("csharp_test_content", fileStream) });

            // Assert
            var submissionId = submissionIds.Single();
            submissionId.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task CreateAsync_ShouldCreateSubmission_FromStreamWithName()
        {
            // Arrange
            await using var fileStream = _dataHelper.Files().GetSampleFileStream();
            var filePath = _dataHelper.Files().GetSampleFilePath();

            // Act
            var submissionIds = await _submissionsClient.CreateAsync(_workflowId, new[] { (filePath, fileStream) });

            // Assert
            var submissionId = submissionIds.Single();
            submissionId.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task CreateAsync_ShouldCreateSubmission_FromFilePath()
        {
            // Arrange
            var filePath = _dataHelper.Files().GetSampleFilePath();

            // Act
            var submissionIds = await _submissionsClient.CreateAsync(_workflowId, paths: new[] { filePath });

            // Assert
            var submissionId = submissionIds.Single();
            submissionId.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task CreateAsync_ShouldCreateSubmission_FromUri()
        {
            // Arrange
            var uri = _dataHelper.Uris().GetSampleUri();

            // Act
            var submissionIds = await _submissionsClient.CreateAsync(_workflowId, uris: new[] { uri });

            // Assert
            var submissionId = submissionIds.Single();
            submissionId.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task CreateAsync_ShouldCreateMultiFileSubmission_FromFilePath()
        {
            // Arrange
            var filePath = _dataHelper.Files().GetSampleFilePath();

            // Act
            var submissionIds = await _submissionsClient.CreateAsync(_workflowId, paths: new[] { filePath, filePath }, default, SubmissionResultsFileVersion.Three, true);

            // Assert
            var submissionId = submissionIds.Single();
            submissionId.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task GetAsync_ShouldFetchSubmission()
        {
            // Arrange
            var submissionId = (await _dataHelper.Submissions().GetAnyAsync(_workflowId)).Id;

            // Act
            var submission = await _submissionsClient.GetAsync(submissionId);

            // Assert
            submission.Id.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task ListAsync_ShouldFetchSubmissions()
        {
            // Arrange
            var listData = await _dataHelper.Submissions().ListAnyAsync(_workflowId);

            // Act
            var submissions = await _submissionsClient.ListAsync(new List<int> { listData.submissionId }, new List<int> { listData.workflowId }, null);
            var submission = submissions.First();

            // Assert
            submissions.Should().HaveCountGreaterThan(0);
            submission.Id.Should().BeGreaterThan(0);
            submission.Status.Should().BeOfType<SubmissionStatus>();
        }


        [Test]
        public async Task ListAsync_ShouldFetchSubmissionsWithCursor()
        {
            // Arrange
            var listData = await _dataHelper.Submissions().ListAnyAsync(_workflowId);

            // Act
            var submissions = await _submissionsClient.ListAsync(new List<int> { listData.submissionId }, new List<int> { listData.workflowId }, null, 0, 1000);

            submissions.Should().NotBeNull();
            submissions.PageInfo.Should().NotBeNull();
            submissions.Data.Should().NotBeNull();
            var submission = submissions.Data.First();

            // Assert
            submissions.Data.Should().HaveCountGreaterThan(0);
            submission.Id.Should().BeGreaterThan(0);
            submission.Status.Should().BeOfType<SubmissionStatus>();
        }


        [Test]
        public async Task ListAsync_SubmissionFilter_ShouldFetchSubmissions()
        {
            // Arrange
            var listData = await _dataHelper.Submissions().ListAnyAsync(_workflowId);

            var filters = new OrFilter
            {
                Or = new List<IFilter>
                {
                    new SubmissionFilter
                    {
                        Status = SubmissionStatus.COMPLETE,
                        Retrieved = false
                    },
                    new SubmissionFilter
                    {
                        Status = SubmissionStatus.FAILED,
                        Retrieved = false
                    }
                }
            };

            // Act
            var submissions = await _submissionsClient.ListAsync(new List<int> { listData.submissionId }, new List<int> { listData.workflowId }, filters, 0, 1000);

            submissions.Should().NotBeNull();
            submissions.PageInfo.Should().NotBeNull();
            submissions.Data.Should().NotBeNull();
            var submission = submissions.Data.First();

            // Assert
            submissions.Data.Should().HaveCountGreaterThan(0);
            submission.Id.Should().BeGreaterThan(0);
            submission.Status.Should().BeOfType<SubmissionStatus>();
        }


        [Test]
        public async Task ListSubmissions_SubmissionFilterReviewInProgress_ShouldFetchSubmissions()
        {
            // Arrange
            var filters = new SubmissionFilter
            {
                ReviewInProgress = false
            };

            // Act
            var submissions = await _submissionsClient.ListAsync(null, new List<int> { _workflowId }, filters, 0, 10);

            // Assert
            submissions.Should().NotBeNull();
            submissions.PageInfo.Should().NotBeNull();
            submissions.Data.Should().NotBeNull();
            submissions.Data.Should().HaveCount(0);

            foreach (Submission submission in submissions.Data)
            {
                submission.Id.Should().BeGreaterThan(0);
                submission.ReviewInProgress.Should().NotBeTrue();
            }
        }


        [Test]
        public async Task ListSubmissions_SubmissionFilterCreatedAt_ShouldFetchSubmissions()
        {
            // Arrange
            var listData = await _dataHelper.Submissions().ListAnyAsync(_workflowId);

            var filters = new SubmissionFilter
            {
                CreatedAt = new DateRangeFilter(){
                    From = "2022-01-01",
                    To = DateTime.Now.ToString("yyyy-MM-dd")
                }
            };

            // Act
            var submissions = await _submissionsClient.ListAsync(null, new List<int> { listData.workflowId }, filters, 0, 10);

            submissions.Should().NotBeNull();
            submissions.PageInfo.Should().NotBeNull();
            submissions.Data.Should().NotBeNull();

            // Assert
            submissions.Data.Should().HaveCountGreaterThan(0);
            foreach (Submission submission in submissions.Data)
            {
                submission.Id.Should().BeGreaterThan(0);
                submission.CreatedAt.Value.Should().BeLessThan(TimeSpan.FromTicks(DateTime.Now.Ticks));
            }
        }


        [Test]
        public async Task ListSubmissions_SubmissionFilterStatus_ShouldFetchSubmissions()
        {
            // Arrange
            var listData = await _dataHelper.Submissions().ListAnyAsync(_workflowId);
            var filters = new SubmissionFilter
            {
                Status = SubmissionStatus.PENDING_AUTO_REVIEW
            };

            // Act
            var submissions = await _submissionsClient.ListAsync(null, new List<int> { listData.workflowId }, filters, 0, 10);

            submissions.Should().NotBeNull();
            submissions.PageInfo.Should().NotBeNull();
            submissions.Data.Should().NotBeNull();
            // Assert
            submissions.Data.Should().HaveCountGreaterThan(0);
            foreach (Submission submission in submissions.Data)
            {
                submission.Id.Should().BeGreaterThan(0);
                submission.Status.Should().BeOfType<SubmissionStatus>();
                submission.Status.Should().Be(SubmissionStatus.PENDING_AUTO_REVIEW);
            }
        }


        [Test]
        public async Task ListSubmissions_SubmissionFilterUpdatedAt_ShouldFetchSubmissions()
        {
            // Arrange
            var listData = await _dataHelper.Submissions().ListAnyAsync(_workflowId);

            var filters = new SubmissionFilter
            {
                UpdatedAt = new DateRangeFilter(){
                    From = "2022-01-01",
                    To = DateTime.Now.ToString("yyyy-MM-dd")
                }
            };

            // Act
            var submissions = await _submissionsClient.ListAsync(null, new List<int> { listData.workflowId }, filters, 0, 10);

            submissions.Should().NotBeNull();
            submissions.PageInfo.Should().NotBeNull();
            submissions.Data.Should().NotBeNull();
            var submission = submissions.Data.First();

            // Assert
            submissions.Data.Should().HaveCountGreaterThan(0);
            submission.Id.Should().BeGreaterThan(0);
            submission.Status.Should().BeOfType<SubmissionStatus>();
        }


        [Test]
        public async Task ListSubmissions_SubmissionOrAndFilter_ShouldFetchSubmissions()
        {
            // Arrange
            var listData = await _dataHelper.Submissions().ListAnyAsync(_workflowId);

            var filters = new OrFilter
            {
                Or = new List<IFilter>
                {
                    new AndFilter
                    {
                        And = new List<IFilter>
                        {
                            new SubmissionFilter
                            {
                                Status = SubmissionStatus.COMPLETE,
                            },
                            new SubmissionFilter
                            {
                                Retrieved = false,
                            }
                        }
                    },
                    new AndFilter
                    {
                        And = new List<IFilter>
                        {
                            new SubmissionFilter
                            {
                                Status = SubmissionStatus.FAILED,
                            },
                            new SubmissionFilter
                            {
                                Retrieved = false,
                            }
                        }
                    }
                }
            };
            // Act
            var submissions = await _submissionsClient.ListAsync(null, new List<int> { listData.workflowId }, filters, 0, 10);

            submissions.Should().NotBeNull();
            submissions.PageInfo.Should().NotBeNull();
            submissions.Data.Should().NotBeNull();
            foreach (var submission in submissions.Data)
            {
                submission.Id.Should().BeGreaterThan(0);
                submission.Status.Should().BeOfType<SubmissionStatus>();
                submission.Status.Should().Match<SubmissionStatus>(status => status == SubmissionStatus.COMPLETE || status == SubmissionStatus.FAILED);
                submission.Retrieved.Should().BeFalse();
            }
        }



        [Test]
        public async Task GenerateSubmissionResultAsync_ShouldReturnJob()
        {
            // Arrange
            var submissionId = (await _dataHelper.Submissions().GetAnyAsync(_workflowId)).Id;
            while (SubmissionStatus.PROCESSING == (await _submissionsClient.GetAsync(submissionId)).Status)
            {
                await Task.Delay(50);
            }

            // Act
            var jobId = await _submissionsClient.GenerateSubmissionResultAsync(submissionId);

            // Assert
            jobId.Should().NotBeEmpty();
        }

        [Test]
        public async Task MarkSubmissionAsRetrieved_ShouldUpdateSubmission()
        {
            // Arrange
            var submissionId = (await _dataHelper.Submissions().GetAnyAsync(_workflowId)).Id;
            while (SubmissionStatus.PROCESSING == (await _submissionsClient.GetAsync(submissionId)).Status)
            {
                await Task.Delay(50);
            }

            // Act
            var submission = await _submissionsClient.MarkSubmissionAsRetrieved(submissionId);
            // Assert
            submission.Should().NotBeNull();

            var result = await _submissionsClient.ListAsync(new List<int>() { submission.Id }, new List<int>(), null);
            result.Should().NotBeNullOrEmpty();

            var updated_sub = result.First();
            updated_sub.Should().NotBeNull();
            updated_sub.Retrieved.Should().BeTrue();

        }
    }
}
