using GraphQL.Common.Request;
using Indico.Entity;
using Indico.Exception;
using Indico.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Indico.Query
{
    public class ListSubmissions : IQuery<List<Submission>>
    {
        private readonly IndicoClient _client;
        public List<int> SubmissionIds { get; set; }
        public List<int> WorkflowIds { get; set; }
        public SubmissionFilter Filters { get; set; } = new SubmissionFilter();
        public int Limit { get; set; } = 1000;

        public ListSubmissions(IndicoClient client) => _client = client;

        public async Task<List<Submission>> Exec(CancellationToken cancellationToken = default)
        {
            string query = @"
                    query ListSubmissions(
                        $submissionIds: [Int],
                        $workflowIds: [Int],
                        $filters: SubmissionFilter,
                        $limit: Int
                    ){
                        submissions(
                            submissionIds: $submissionIds,
                            workflowIds: $workflowIds,
                            filters: $filters,
                            limit: $limit
                        ){
                           submissions {
                                id
                                datasetId
                                workflowId
                                status
                                inputFile
                                inputFilename
                                resultFile
                            }
                        }
                    }
                ";
            var request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "ListSubmissions",
                Variables = new
                {
                    submissionIds = SubmissionIds,
                    workflowIds = WorkflowIds,
                    filters = Filters.ToAnonymousType(),
                    limit = Limit
                }
            };

            var response = await _client.GraphQLHttpClient.SendQueryAsync(request, cancellationToken);
            
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var subs = (JArray)response.Data.submissions.submissions;
            var submissions = subs.Select(submission => new Submission()
            {
                Id = submission.Value<int>("id"),
                DatasetId = submission.Value<int>("datasetId"),
                WorkflowId = submission.Value<int>("workflowId"),
                Status = (SubmissionStatus)Enum.Parse(typeof(SubmissionStatus), submission.Value<string>("status")),
                InputFile = submission.Value<string>("inputFile"),
                InputFilename = submission.Value<string>("inputFilename"),
                ResultFile = submission.Value<string>("resultFile"),
                Retrieved = submission.Value<bool>("retrieved"),
                Errors = submission.Value<string>("errors")

            }).ToList();

            return submissions;
        }
    }
}
