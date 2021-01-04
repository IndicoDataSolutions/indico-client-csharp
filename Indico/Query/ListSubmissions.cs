using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Entity;
using Indico.Exception;
using Indico.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Indico.Query
{
    public class ListSubmissions : Query<List<Submission>>
    {
        IndicoClient _client;
        public List<int> SubmissionIds { get; set; }
        public List<int> WorkflowIds { get; set; }
        public SubmissionFilter Filters { get; set; } = new SubmissionFilter();
        public int Limit { get; set; } = 1000;

        public ListSubmissions(IndicoClient client) => this._client = client;

        async public Task<List<Submission>> Exec()
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
            GraphQLRequest request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "ListSubmissions",
                Variables = new
                {
                    submissionIds = this.SubmissionIds,
                    workflowIds = this.WorkflowIds,
                    filters = this.Filters.ToAnonymousType(),
                    limit = this.Limit
                }
            };

            GraphQLResponse response = await this._client.GraphQLHttpClient.SendQueryAsync(request);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            JArray subs = (JArray)response.Data.submissions.submissions;
            List<Submission> submissions = subs.Select(submission => new Submission()
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
