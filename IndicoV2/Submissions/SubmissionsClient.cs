using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.CommonModels.Pagination;
using IndicoV2.StrawberryShake;
using IndicoV2.Submissions.Models;
using IndicoV2.Reviews.Models;
using IndicoV2.Storage;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace IndicoV2.Submissions
{
    public class SubmissionsClient : ISubmissionsClient
    {
        private readonly IndicoStrawberryShakeClient _strawberryShakeClient;
        private readonly IndicoClient _indicoClient;

        public SubmissionsClient(IndicoClient indicoClient)
        {
            _indicoClient = indicoClient;
            _strawberryShakeClient = indicoClient.IndicoStrawberryShakeClient;
        }

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<Stream> streams, CancellationToken cancellationToken = default, SubmissionResultsFileVersion? resultsFileVersion = null, bool bundle = false)
        {
            var uploadRequest = new UploadStream(_indicoClient)
            {
                Streams = streams.ToList()
            };
            var fileMetadata = await uploadRequest.Call();
            var files = new List<object>();
            foreach (JObject uploadMeta in fileMetadata)
            {
                var meta = new JObject
                    {
                        { "name", uploadMeta.Value<string>("name") },
                        { "path", uploadMeta.Value<string>("path") },
                        { "upload_type", uploadMeta.Value<string>("upload_type") }
                    };

                var file = new
                {
                    filename = uploadMeta.Value<string>("name"),
                    filemeta = meta.ToString()
                };

                files.Add(file);
            }
            return await _strawberryShakeClient.Submissions().Create(workflowId, (IEnumerable<(string Name, string Meta)>)files, cancellationToken, (SubmissionResultVersion?)resultsFileVersion, bundle);
        }

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<(string Name, Stream Content)> filesToUpload, CancellationToken cancellationToken = default, SubmissionResultsFileVersion? resultsFileVersion = null, bool bundle = false, int batchSize = 20)
        {
            _indicoClient.Logger.LogDebug($"IndicoV2.Submission.SubmissionsClient.CreateAsync(): submitting streams to workflow {workflowId} (uploading files)");
            var filesUploaded = await _indicoClient.Storage().UploadAsync(filesToUpload, cancellationToken, batchSize: batchSize);
            _indicoClient.Logger.LogDebug($"IndicoV2.Submission.SubmissionsClient.CreateAsync(): submitting streams to workflow {workflowId} (creating submission)");
            var result =  await _strawberryShakeClient.Submissions().Create(workflowId, filesUploaded, cancellationToken, (SubmissionResultVersion?)resultsFileVersion, bundle);
            _indicoClient.Logger.LogDebug($"IndicoV2.Submission.SubmissionsClient.CreateAsync(): submitted streams to workflow {workflowId}");
            return result;
        }


        [Obsolete("This is the Legacy version and will be deprecated. Please use CreateAsync instead.")]
        public Task<IEnumerable<int>> CreateAsyncLegacy(int workflowId, IEnumerable<string> paths,
          CancellationToken cancellationToken) =>
          CreateAsync(workflowId, paths, cancellationToken);

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<Uri> uris,
            CancellationToken cancellationToken = default, SubmissionResultsFileVersion? resultsFileVersion = null, bool bundle = false) =>
            await _strawberryShakeClient.Submissions().CreateUri(workflowId, uris, cancellationToken);

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<string> paths,
          CancellationToken cancellationToken, SubmissionResultsFileVersion? resultsFileVersion = null, bool bundle = false, int batchSize = 20)
        {
            var filesToUpload = new List<(string Name, Stream content)>();
            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                    filesToUpload.Add((path, fileStream));
                }
                else
                {
                    throw new ArgumentException($"Cannot find a file at path {path}");
                }

            }
            return await CreateAsync(workflowId, filesToUpload: filesToUpload, cancellationToken, resultsFileVersion, bundle, batchSize);
        }

        [Obsolete("This is the Legacy version and will be deprecated. Please use ListAsync instead.")]
        public async Task<IEnumerable<ISubmission>> ListAsync(IEnumerable<int> submissionIds, IEnumerable<int> workflowIds, IFilter filters, int limit = 1000,
            CancellationToken cancellationToken = default) => (await ListAsync(submissionIds, workflowIds, filters, null, limit, cancellationToken)).Data;

        public async Task<IHasCursor<IEnumerable<ISubmission>>> ListAsync(IEnumerable<int> submissionIds, IEnumerable<int> workflowIds, IFilter filters, int? after, int limit = 1000, CancellationToken cancellationToken = default)
        {
            _indicoClient.Logger.LogDebug("IndicoV2.Submission.SubmissionsClient.ListAsync(): getting submissions list");
            var ssFilters = filters != null ? FilterConverter.ConvertToSs(filters) : null;

            var readonlyIds = submissionIds == null ? (IReadOnlyList<int?>)new List<int?>() : submissionIds.Select(x => (int?)x).ToList().AsReadOnly();
            var readonlyWorkflowIds = workflowIds == null ? (IReadOnlyList<int?>)new List<int?>() : workflowIds.Select(x => (int?)x).ToList().AsReadOnly();
            var result = await _strawberryShakeClient.Submissions().List(readonlyIds, readonlyWorkflowIds, ssFilters, limit, after, cancellationToken);
            _indicoClient.Logger.LogDebug("IndicoV2.Submission.SubmissionsClient.ListAsync(): got submissions list");

            return new HasCursor<IEnumerable<ISubmission>>()
            {
                Data = result?.Submissions?.Select(x => ToSubmissionFromSs(x)).ToList() ?? new List<ISubmission>(),
                PageInfo = new PageInfo()
                {
                    HasNextPage = result?.PageInfo?.HasNextPage ?? false,
                    StartCursor = result?.PageInfo?.StartCursor ?? 0,
                    EndCursor = result?.PageInfo?.EndCursor ?? 0,
                    AggregateCount = result?.PageInfo?.AggregateCount ?? 0

                }
            };
        }

        public async Task<ISubmission> GetAsync(int submissionId, CancellationToken cancellationToken = default)
        {
            var result = await _strawberryShakeClient.Submissions().Get(submissionId, cancellationToken);
            if (!Enum.IsDefined(typeof(StrawberryShake.SubmissionStatus), result.Status))
            {
                throw new NotSupportedException($"Cannot read submission status: {result.Status}");
            }
            return GetSubmissionToSubmission(result);
        }


        public async Task<string> GenerateSubmissionResultAsync(int submissionId, CancellationToken cancellationToken = default) =>
            await _strawberryShakeClient.Submissions().GenerateSubmissionResult(submissionId, cancellationToken);

        public async Task<ISubmission> MarkSubmissionAsRetrieved(int submissionId, bool retrieved = true, CancellationToken cancellationToken = default)
        {
            _indicoClient.Logger.LogDebug($"IndicoV2.Submission.SubmissionsClient.MarkSubmissionAsRetrieved(): marking submission {submissionId} retrieved");
            await _strawberryShakeClient.Submissions().MarkRetrieved(submissionId, retrieved, cancellationToken);
            var result = await _strawberryShakeClient.Submissions().Get(submissionId, cancellationToken);
            _indicoClient.Logger.LogDebug($"IndicoV2.Submission.SubmissionsClient.MarkSubmissionAsRetrieved(): marked submission {submissionId} retrieved");
            return GetSubmissionToSubmission(result);
        }

        private ISubmission ToSubmissionFromSs(IListSubmissions_Submissions_Submissions submission) => new SubmissionSs(submission);

        private ISubmission GetSubmissionToSubmission(IGetSubmission_Submission result) => new Submission
        {
            Id = result.Id ?? 0,
            Status = (Models.SubmissionStatus)result.Status,
            DatasetId = result.DatasetId ?? 0,
            WorkflowId = result.WorkflowId ?? 0,
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt,
            CompletedAt = result.CompletedAt,
            FilesDeleted = result.FilesDeleted,
            InputFiles = result.InputFiles.Select(inputFile => new SubmissionFile
            {
                Id = inputFile.Id,
                FilePath = inputFile.Filepath,
                FileName = inputFile.Filename,
                FileType = inputFile.Filetype.ToString(),
                SubmissionId = inputFile.SubmissionId,
                FileSize = inputFile.FileSize,
                NumPages = inputFile.NumPages
            }).ToArray(),
            InputFile = result.InputFile,
            InputFilename = result.InputFilename,
            ResultFile = result.ResultFile,
            OutputFiles = result.OutputFiles.Select(x => new SubmissionOutput() { }).ToArray(),
            Retrieved = result.Retrieved ?? throw new ArgumentException("Invalid value for retrieved received from call"),
            AutoReview = result.AutoReview != null ? new Review
            {
                Id = result.AutoReview.Id,
                SubmissionId = result.AutoReview.SubmissionId,
                CreatedAt = result.AutoReview.CreatedAt,
                CreatedBy = result.AutoReview.CreatedBy,
                StartedAt = result.AutoReview.StartedAt,
                CompletedAt = result.AutoReview.CompletedAt,
                Rejected = result.AutoReview.Rejected,
                ReviewType = (Models.ReviewType)result.AutoReview.ReviewType,
                Notes = result.AutoReview.Notes,
            } : new Review() { },
            Retries = result.Retries.Select(submissionRetry => new SubmissionRetry
            {
                Id = submissionRetry.Id,
                SubmissionId = submissionRetry.SubmissionId,
                PreviousErrors = submissionRetry.PreviousErrors,
                PreviousStatus = (Models.SubmissionStatus)submissionRetry.PreviousStatus,
                RetryErrors = submissionRetry.RetryErrors
            }).ToArray(),
            Reviews = result.Reviews.Select(review => new Review
            {
                Id = review.Id,
                SubmissionId = review.SubmissionId,
                CreatedAt = review.CreatedAt,
                CreatedBy = review.CreatedBy,
                StartedAt = review.StartedAt,
                CompletedAt = review.CompletedAt,
                Rejected = review.Rejected,
                ReviewType = (Models.ReviewType)review.ReviewType,
                Notes = review.Notes,
            }).ToArray(),
            ReviewInProgress = result.ReviewInProgress,
            Errors = result.Errors ?? null
        };
    }
}
