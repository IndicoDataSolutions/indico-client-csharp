using System.Threading;
using System.Threading.Tasks;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Exception;
using Indico.Jobs;
using Newtonsoft.Json.Linq;

namespace Indico.Mutation
{
    public class GenerateSubmissionResult : Mutation<Job>
    {
        IndicoClient _client;
        public int SubmissionId { get; set; }

        public GenerateSubmissionResult(IndicoClient client) => this._client = client;

        async public Task<Job> Exec(CancellationToken cancellationToken = default)
        {
            string query = @"
                    mutation CreateSubmissionResults($submissionId: Int!) {
                        submissionResults(submissionId: $submissionId) {
                            jobId
                        }
                    }
                ";

            GraphQLRequest request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "CreateSubmissionResults",
                Variables = new
                {
                    submissionId = this.SubmissionId
                }
            };

            GraphQLResponse response = await this._client.GraphQLHttpClient.SendMutationAsync(request, cancellationToken);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            JObject submissionResults = response.Data.submissionResults;
            string jobId = submissionResults.Value<string>("jobId");
            return new Job(this._client.GraphQLHttpClient, jobId);
        }
    }
}

