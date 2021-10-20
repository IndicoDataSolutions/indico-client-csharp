using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using Indico.Entity;
using Indico.Exception;
using Indico.Types;
using Newtonsoft.Json.Linq;

namespace Indico.Mutation
{


    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    /// <summary>
    /// Updates Submission.
    /// </summary>
    public class UpdateSubmission : IMutation<Submission>
    {
        private readonly IndicoClient _client;
        private int? _submissionId;

        /// <summary>
        /// Submission id.
        /// </summary>
        public int SubmissionId
        {
            get
            {
                if (!_submissionId.HasValue)
                {
                    throw new ArgumentNullException(nameof(SubmissionId));
                }

                return _submissionId.Value;
            }

            set => _submissionId = value;
        }

        /// <summary>
        /// If retrieved.
        /// </summary>
        public bool Retrieved { get; set; }

        /// <summary>
        /// Update Submission Constructor.
        /// </summary>
        /// <param name="client">Client used to send API requests.</param>
        public UpdateSubmission(IndicoClient client) => _client = client;

        /// <summary>
        /// Executes query and returns Submission.
        /// </summary>
        public async Task<Submission> Exec(CancellationToken cancellationToken = default)
        {
            var query = @"
                    mutation UpdateSubmission($submissionId: Int!, $retrieved: Boolean) {
                        updateSubmission(submissionId: $submissionId, retrieved: $retrieved) {
                            id
                            datasetId
                            workflowId
                            status
                            inputFile
                            inputFilename
                            resultFile
                            retrieved
                            errors
                        }
                    }
                ";

            var request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "UpdateSubmission",
                Variables = new
                {
                    submissionId = SubmissionId,
                    retrieved = Retrieved   
                }
            };

            var response = await _client.GraphQLHttpClient.SendMutationAsync<dynamic>(request, cancellationToken);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            JObject submission = response.Data.updateSubmission;
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
