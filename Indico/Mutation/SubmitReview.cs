using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Exception;
using Indico.Jobs;
using Newtonsoft.Json.Linq;

namespace Indico.Mutation
{
    public class SubmitReview : Mutation<Job>
    {
        IndicoClient _client;
        public int SubmissionId { get; set; }
        public JObject Changes { get; set; }
        public bool Rejected { get; set; } = false;
        public bool? ForceComplete { get; set; }
                
        public SubmitReview(IndicoClient client) => this._client = client;

        async public Task<Job> Exec()
        {
            if (Changes == null && !Rejected)
                throw new RuntimeException("Must provide Changes or Reject=true");

            var args = new Dictionary<string, string>
            {
                {"submissionId", "Int!"},
                {"changes", "JSONString"},
                {"rejected", "Boolean"}
            };

            dynamic vars = new ExpandoObject();
            vars.submissionId = SubmissionId;
            vars.changes = Changes.ToString();
            vars.rejected = Rejected;

            if (ForceComplete != null)
            {
                args.Add("forceComplete", "Boolean");
                vars.forceComplete = ForceComplete;
            }

            string queryArgs = string.Join(", ", args.Select(pair => $"${pair.Key}: {pair.Value}"));
            string autoReviewArgs = string.Join(", ", args.Select(pair => $"{pair.Key}: ${pair.Key}"));

            string query = $@"
                mutation SubmitReview({queryArgs}) {{
                    submitAutoReview({autoReviewArgs}) {{
                        jobId
                    }}
                }}
            ";

            GraphQLRequest request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "SubmitReview",
                Variables = vars
            };

            GraphQLResponse response = await this._client.GraphQLHttpClient.SendMutationAsync(request);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var submitAutoReview = response.Data.submitAutoReview;
            string jobId = (string)submitAutoReview.jobId;
            
            return new Job(this._client.GraphQLHttpClient, jobId);
        }
    }
}