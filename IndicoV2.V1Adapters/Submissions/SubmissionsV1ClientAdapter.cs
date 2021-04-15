using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using Indico;
using Indico.Exception;
using Indico.Mutation;
using Indico.Query;
using IndicoV2.Storage;
using IndicoV2.Submissions;
using IndicoV2.Submissions.Models;
using IndicoV2.V1Adapters.Converters;
using IndicoV2.V1Adapters.Submissions.Models;
using Newtonsoft.Json.Linq;

namespace IndicoV2.V1Adapters.Submissions
{
    public class SubmissionsV1ClientAdapter : ISubmissionsClient
    {
        private readonly IndicoClient _indicoClient;
        private readonly IStorageClient _storage;

        public SubmissionsV1ClientAdapter(IndicoClient indicoClient, IStorageClient storage)
        {
            _indicoClient = indicoClient;
            _storage = storage;
        }

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<(string Name, Stream Content)> filesToUpload, CancellationToken cancellationToken = default)
        {
            var filesUploaded = await _storage.UploadAsync(filesToUpload, cancellationToken);
            var query = @"
                    mutation WorkflowSubmission($workflowId: Int!, $files: [FileInput]!, $recordSubmission: Boolean) {
                        workflowSubmission(workflowId: $workflowId, files: $files, recordSubmission: $recordSubmission) {
                            jobIds
                            submissionIds
                        }
                    }
                ";
            var args = new
            {
                workflowId,
                files = filesUploaded.Select(f => new
                {
                    filename = f.Name,
                    filemeta = f.Meta,
                }).ToArray(),
            };
            var result = await _indicoClient.GraphQLHttpClient.SendMutationAsync<JObject>(new GraphQLRequest(query, args, "WorkflowSubmission"), cancellationToken);

            if (result.Errors != null && result.Errors.Any())
            {
                throw new GraphQLException(result.Errors);
            }

            return result.Data["workflowSubmission"]["submissionIds"].Values<int>();
        }

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<Stream> streams, CancellationToken cancellationToken = default)
        {
            var submissionMutation = new WorkflowSubmission(_indicoClient) { Streams = streams.ToList(), WorkflowId = workflowId };
            var submissionIds = await submissionMutation.Exec(cancellationToken);

            return submissionIds;
        }

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<Uri> uris, CancellationToken cancellationToken = default)
        {
            var submissionMutation = new WorkflowSubmission(_indicoClient) { WorkflowId = workflowId, Urls = uris.Select(u => u.ToString()).ToList() };
            var submissionIds = await submissionMutation.Exec(cancellationToken);

            return submissionIds;
        }

        public async Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<string> paths, CancellationToken cancellationToken = default)
        {
            var submissionMutation = new WorkflowSubmission(_indicoClient) { WorkflowId = workflowId, Files = paths.ToList() };
            var submissionIds = await submissionMutation.Exec(cancellationToken);

            return submissionIds;
        }

        public async Task<IEnumerable<ISubmission>> ListAsync(IEnumerable<int> submissionIds, IEnumerable<int> workflowIds, IFilter filters, int limit = 1000, CancellationToken cancellationToken = default)
        {
            var listSubmissionQuery = new ListSubmissions(_indicoClient)
            {
                SubmissionIds = submissionIds?.ToList(),
                WorkflowIds = workflowIds?.ToList(),
                Filters = filters != null ? filters.ConvertToLegacy() : new Indico.Entity.SubmissionFilter(),
                Limit = limit
            };

            var submissions = await listSubmissionQuery.Exec(cancellationToken);

            return submissions.Select(s => new V1SubmissionAdapter(s));
        }

        public async Task<ISubmission> GetAsync(int submissionId, CancellationToken cancellationToken = default)
        {
            var submission = await new GetSubmission(_indicoClient) { Id = submissionId }.Exec(cancellationToken);
            return new V1SubmissionAdapter(submission);
        }

        public async Task<string> GenerateSubmissionResultAsync(int submissionId, CancellationToken cancellationToken)
        {
            var job = await new GenerateSubmissionResult(_indicoClient) { SubmissionId = submissionId }.Exec(cancellationToken);

            return job.Id;
        }
    }
}
