using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Indico.Entity;
using IndicoV2.CommonModels.Pagination;
using IndicoV2.StrawberryShake;
using IndicoV2.Submissions.Models;
using IndicoV2.V1Adapters.Submissions;

namespace IndicoV2.Submissions
{
    public class SubmissionsClient : ISubmissionsClient
    {
        private readonly SubmissionsV1ClientAdapter _legacy;
        private readonly IndicoStrawberryShakeClient _strawberryShakeClient;
        private readonly IndicoClient _indicoClient;

        public SubmissionsClient(IndicoClient indicoClient)
        {
            _indicoClient = indicoClient;
            _legacy = new SubmissionsV1ClientAdapter(indicoClient.LegacyClient);
            _strawberryShakeClient = indicoClient.IndicoStrawberryShakeClient;
        }

       
        public Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<Stream> streams,
            CancellationToken cancellationToken = default) =>
            _legacy.CreateAsync(workflowId, streams, cancellationToken);

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<(string Name, Stream Content)> filesToUpload,  CancellationToken cancellationToken = default, SubmissionResultsFileVersion? resultsFileVersion = null)
        {
            var filesUploaded = await _indicoClient.Storage().UploadAsync(filesToUpload, cancellationToken);
            return await _strawberryShakeClient.Submissions().Create(workflowId, filesUploaded, cancellationToken, (SubmissionResultVersion?)resultsFileVersion);
        }


     
        public Task<IEnumerable<int>> CreateAsyncLegacy(int workflowId, IEnumerable<string> paths,
          CancellationToken cancellationToken) =>          
          _legacy.CreateAsync(workflowId, paths, cancellationToken);
        
        public Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<Uri> uris,
            CancellationToken cancellationToken = default, SubmissionResultsFileVersion? resultsFileVersion = null) =>
            _legacy.CreateAsync(workflowId, uris, cancellationToken);

         Task<IEnumerable<int>> ISubmissionsClient.CreateAsync(int workflowId, IEnumerable<string> paths,
           CancellationToken cancellationToken, SubmissionResultsFileVersion? resultsFileVersion = null) {
            var filesToUpload = new List<(string Name, Stream content)>();
            foreach(var path in paths)
            {
                if(File.Exists(path))
                {
                    var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                    filesToUpload.Add((path, fileStream));
                }
                else
                {
                    throw new ArgumentException($"Cannot find a file at path {path}");
                } 

            }
             return CreateAsync(workflowId, filesToUpload, cancellationToken, resultsFileVersion);
        }


        public Task<IEnumerable<ISubmission>> ListAsync(IEnumerable<int> submissionIds, IEnumerable<int> workflowIds, IFilter filters, int limit = 1000,
            CancellationToken cancellationToken = default) => _legacy.ListAsync(submissionIds, workflowIds, filters, limit, cancellationToken);


        public async Task<IHasCursor<IEnumerable<ISubmission>>> ListAsync(IEnumerable<int> submissionIds, IEnumerable<int> workflowIds, IFilter filters, int? after, int limit = 1000, CancellationToken cancellationToken = default)
        {
            var ssFilters = filters != null ? FilterConverter.ConvertToSs(filters) : null;

            var readonlyIds = (IReadOnlyList<int?>)submissionIds.Select(x => (int?)x).ToList().AsReadOnly();
            var readonlyWorkflowIds = (IReadOnlyList<int?>)workflowIds.Select(x => (int?)x).ToList().AsReadOnly();
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


        public Task<ISubmission> GetAsync(int submissionId, CancellationToken cancellationToken = default) =>
            _legacy.GetAsync(submissionId, cancellationToken);

        public Task<string> GenerateSubmissionResultAsync(int submissionId, CancellationToken cancellationToken = default) =>
            _legacy.GenerateSubmissionResultAsync(submissionId, cancellationToken);

        public async Task<ISubmission> MarkSubmissionAsRetrieved(int submissionId, bool retrieved = true, CancellationToken cancellationToken = default) => await _legacy.UpdateSubmissionAsync(submissionId, retrieved, cancellationToken);

        private ISubmission ToSubmissionFromSs(IListSubmissions_Submissions_Submissions submission) => new SubmissionSs(submission);
       
    }
}
