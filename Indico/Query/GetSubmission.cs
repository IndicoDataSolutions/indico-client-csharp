using GraphQL.Common.Request;
using Indico.Entity;
using Indico.Exception;
using Indico.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Indico.Query
{
    /// <summary>
    /// Gets submission.
    /// </summary>
    public class GetSubmission : IQuery<Submission>
    {
        private readonly IndicoClient _client;
        private int? _submissionId;

        /// <summary>
        /// Submission id.
        /// </summary>
        public int Id 
        {
            get
            {
                if (!_submissionId.HasValue)
                {
                    throw new ArgumentNullException();
                }

                return _submissionId.Value;
            }

            set => _submissionId = value;
        }

        /// <summary>
        /// GetSubmission constructor.
        /// </summary>
        /// <param name="client">Client used to send API requests.</param>
        public GetSubmission(IndicoClient client) => _client = client;

        /// <summary>
        /// Queries the server and returns Submission
        /// </summary>
        /// <returns>Submission</returns>
        public async Task<Submission> Exec(CancellationToken cancellationToken)
        {
            var query = @"
                    query GetSubmission($submissionId: Int!){
                        submission(id: $submissionId){
                            id
                            datasetId
                            workflowId
                            status
                            inputFile
                            inputFilename
                            resultFile
                            retrieved
                        }
                    }
                ";
            var request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "GetSubmission",
                Variables = new
                {
                    submissionId = Id
                }
            };

            var response = await _client.GraphQLHttpClient.SendQueryAsync(request, cancellationToken);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            JObject submission = response.Data.submission;
            return new Submission()
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
            };
        }
    }
}
