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
    /// <summary>
    /// Lists submissions.
    /// </summary>
    public class ListSubmissions : IQuery<List<Submission>>
    {
        private readonly IndicoClient _client;

        /// <summary>
        /// Ids of submissions to list.
        /// </summary>
        public List<int> SubmissionIds { get; set; }

        /// <summary>
        /// Ids of workflow to list submissions from.
        /// </summary>
        public List<int> WorkflowIds { get; set; }

        /// <summary>
        /// Submission filters.
        /// </summary>
        public SubmissionFilter Filters { get; set; } = new SubmissionFilter();

        /// <summary>
        /// Return list count limit.
        /// </summary>
        /// <value>Default and max is 1000.</value>
        public int Limit { get; set; } = 1000;

        /// <summary>
        /// ListSubmissions constructor.
        /// </summary>
        /// <param name="client">Client used to send API requests.</param>
        public ListSubmissions(IndicoClient client) => _client = client;

        /// <summary>
        /// Executes query and returns list of submissions.
        /// </summary>
        public async Task<List<Submission>> Exec(CancellationToken cancellationToken = default)
        {
            var query = @"
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
