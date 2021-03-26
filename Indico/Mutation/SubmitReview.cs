using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using Indico.Exception;
using Indico.Jobs;
using Newtonsoft.Json.Linq;

namespace Indico.Mutation
{
    /// <summary>
    /// Submits review.
    /// </summary>
    public class SubmitReview : IMutation<Job>
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
        /// Review's changes.
        /// </summary>
        public JObject Changes { get; set; }

        /// <summary>
        /// If review rejected.
        /// </summary>
        public bool Rejected { get; set; } = false;

        /// <summary>
        /// Force complete review.
        /// </summary>
        public bool? ForceComplete { get; set; }

        /// <summary>
        /// Submit Review Constructor.
        /// </summary>
        /// <param name="client">Client used to send API requests.</param>
        public SubmitReview(IndicoClient client) => _client = client;

        /// <summary>
        /// Submits review and returns job.
        /// </summary>
        public async Task<Job> Exec(CancellationToken cancellationToken)
        {
            if (Changes == null && !Rejected)
            {
                throw new ArgumentException($"Provide {nameof(Changes)} or set {nameof(Rejected)} to true.");
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

            var response = await _client.GraphQLHttpClient.SendMutationAsync<dynamic>(request, cancellationToken);
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