using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.CommonModels.Pagination;
using IndicoV2.StrawberryShake;
using IndicoV2.Submissions.Models;
using IndicoV2.Storage;
using Newtonsoft.Json.Linq;

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
            return await _strawberryShakeClient.Submissions().Create(workflowId, (IEnumerable<(string Name, string Meta)>)files, cancellationToken, (SubmissionResultVersion?) resultsFileVersion, bundle);
        }

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<(string Name, Stream Content)> filesToUpload, CancellationToken cancellationToken = default, SubmissionResultsFileVersion? resultsFileVersion = null, bool bundle = false)
        {
            var filesUploaded = await _indicoClient.Storage().UploadAsync(filesToUpload, cancellationToken);
            return await _strawberryShakeClient.Submissions().Create(workflowId, filesUploaded, cancellationToken, (SubmissionResultVersion?)resultsFileVersion, bundle);
        }


        [Obsolete("This is the Legacy version and will be deprecated. Please use CreateAsync instead.")]
        public Task<IEnumerable<int>> CreateAsyncLegacy(int workflowId, IEnumerable<string> paths,
          CancellationToken cancellationToken) =>
          CreateAsync(workflowId, paths, cancellationToken);

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<Uri> uris,
            CancellationToken cancellationToken = default, SubmissionResultsFileVersion? resultsFileVersion = null, bool bundle = false) =>
            await _strawberryShakeClient.Submissions().CreateUri(workflowId, uris, cancellationToken);

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<string> paths,
          CancellationToken cancellationToken, SubmissionResultsFileVersion? resultsFileVersion = null, bool bundle = false)
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
            return await CreateAsync(workflowId, filesToUpload: filesToUpload, cancellationToken, resultsFileVersion, bundle);
        }

        [Obsolete("This is the Legacy version and will be deprecated. Please use ListAsync instead.")]
        public async Task<IEnumerable<ISubmission>> ListAsync(IEnumerable<int> submissionIds, IEnumerable<int> workflowIds, IFilter filters, int limit = 1000,
            CancellationToken cancellationToken = default)
        {
            var result = await ListAsync(submissionIds, workflowIds, filters, null, limit, cancellationToken);
            return result.Data;
        }


        public async Task<IHasCursor<IEnumerable<ISubmission>>> ListAsync(IEnumerable<int> submissionIds, IEnumerable<int> workflowIds, IFilter filters, int? after, int limit = 1000, CancellationToken cancellationToken = default)
        {
            var ssFilters = filters != null ? FilterConverter.ConvertToSs(filters) : null;

            var readonlyIds = (IReadOnlyList<int?>)submissionIds.Select(x => (int?)x).ToList().AsReadOnly();
            var readonlyWorkflowIds = (IReadOnlyList<int?>)workflowIds.Select(x => (int?)x).ToList().AsReadOnly();
            Console.WriteLine("asdfsafasffddfdfdfddfd");
            var result = await _strawberryShakeClient.Submissions().List(readonlyIds, readonlyWorkflowIds, ssFilters, limit, after, cancellationToken);

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
            return new Submission
            {
                Id = result.Id ?? 0,
                Status = (Models.SubmissionStatus) result.Status,
                DatasetId = result.DatasetId ?? 0,
                WorkflowId = result.WorkflowId ?? 0,
                InputFile = result.InputFile,
                InputFilename = result.InputFilename,
                ResultFile = result.ResultFile,
                Retrieved = result.Retrieved ?? throw new ArgumentException("Invalid value for retrieved received from call"),
                Errors = result.Errors ?? null
            };
        }


        public async Task<string> GenerateSubmissionResultAsync(int submissionId, CancellationToken cancellationToken = default) =>
            await _strawberryShakeClient.Submissions().GenerateSubmissionResult(submissionId, cancellationToken);

        public async Task<ISubmission> MarkSubmissionAsRetrieved(int submissionId, bool retrieved = true, CancellationToken cancellationToken = default)
        {
            var resultId = await _strawberryShakeClient.Submissions().MarkRetrieved(submissionId, retrieved, cancellationToken);
            var result = await _strawberryShakeClient.Submissions().List(new List<int?>(submissionId).AsReadOnly(), default, default, default, default, cancellationToken);
            return new SubmissionSs(result?.Submissions?[0]);
        }

        private ISubmission ToSubmissionFromSs(IListSubmissions_Submissions_Submissions submission) => new SubmissionSs(submission);

    }
}
