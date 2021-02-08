using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Common.Request;
using Indico.Exception;
using Indico.Jobs;
using Newtonsoft.Json.Linq;

namespace Indico.Mutation
{
    public class SubmitReview : IMutation<Job>
    {
        private readonly IndicoClient _client;
        public int SubmissionId { get; set; }
        public JObject Changes { get; set; }
        public bool Rejected { get; set; } = false;
        public bool? ForceComplete { get; set; }

        public SubmitReview(IndicoClient client) => _client = client;

        public async Task<Job> Exec(CancellationToken cancellationToken)
        {
            if (Changes == null && !Rejected)
            {
                throw new RuntimeException("Must provide Changes or Reject=true");
            }

            var args = new Dictionary<string, string>
            {
                {"submissionId", "Int!"},
                {"changes", "JSONString"},
                {"rejected", "Boolean"}
            };

            dynamic vars = new ExpandoObject();
            vars.submissionId = SubmissionId;
            vars.changes = Changes?.ToString();
            vars.rejected = Rejected;

            if (ForceComplete != null)
            {
                args.Add("forceComplete", "Boolean");
                vars.forceComplete = ForceComplete;
            }

            var queryArgs = string.Join(", ", args.Select(pair => $"${pair.Key}: {pair.Value}"));
            var autoReviewArgs = string.Join(", ", args.Select(pair => $"{pair.Key}: ${pair.Key}"));

            var query = $@"
                mutation SubmitReview({queryArgs}) {{
                    submitAutoReview({autoReviewArgs}) {{
                        jobId
                    }}
                }}
            ";

            var request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "SubmitReview",
                Variables = vars
            };

            var response = await _client.GraphQLHttpClient.SendMutationAsync(request, cancellationToken);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var submitAutoReview = response.Data.submitAutoReview;
            var jobId = (string)submitAutoReview.jobId;

            return new Job(_client.GraphQLHttpClient, jobId);
        }
    }
}