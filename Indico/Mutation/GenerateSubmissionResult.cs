using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using Indico.Exception;
using Indico.Jobs;
using Newtonsoft.Json.Linq;

namespace Indico.Mutation
{

    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    /// <summary>
    /// Generates submission results.
    /// </summary>
    public class GenerateSubmissionResult : IMutation<Job>
    {
        private readonly IndicoClient _client;

        private int? _submissionId;
        
        /// <summary>
        /// Submission Id.
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
        /// Generate Submission Result Constructor.
        /// </summary>
        /// <param name="client">Client used to send API requests.</param>
        public GenerateSubmissionResult(IndicoClient client) => _client = client;

        /// <summary>
        /// Executes requests and returns <c><see cref="Job"/></c>.
        /// </summary>
        /// <param name="cancellationToken">Token to abort operations.</param>
        /// <returns><c><see cref="Job"/></c></returns>
        public async Task<Job> Exec(CancellationToken cancellationToken = default)
        {
            var query = @"
                    mutation CreateSubmissionResults($submissionId: Int!) {
                        submissionResults(submissionId: $submissionId) {
                            jobId
                        }
                    }
                ";

            var request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "CreateSubmissionResults",
                Variables = new
                {
                    submissionId = SubmissionId
                }
            };

            var response = await _client.GraphQLHttpClient.SendMutationAsync<dynamic>(request, cancellationToken);

            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            JObject submissionResults = response.Data.submissionResults;
            var jobId = submissionResults.Value<string>("jobId");

            return new Job(_client.GraphQLHttpClient, jobId);
        }
    }
}

