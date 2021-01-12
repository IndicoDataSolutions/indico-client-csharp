using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Entity;
using Indico.Exception;
using Indico.Types;
using Newtonsoft.Json.Linq;

namespace Indico.Mutation
{
    public class UpdateSubmission : Mutation<Submission>
    {
        IndicoClient _client;
        public int SubmissionId { get; set; }
        public bool Retrieved { get; set; }

        public UpdateSubmission(IndicoClient client) => this._client = client;

        async public Task<Submission> Exec(CancellationToken cancellationToken = default)
        {
            string query = @"
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
                        }
                    }
                ";

            GraphQLRequest request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "UpdateSubmission",
                Variables = new
                {
                    submissionId = this.SubmissionId,
                    retrieved = this.Retrieved   
                }
            };

            GraphQLResponse response = await this._client.GraphQLHttpClient.SendMutationAsync(request, cancellationToken);
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
