using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Reviews.Models;
using IndicoV2.StrawberryShake;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace IndicoV2.Reviews
{
    public class ReviewsClient : IReviewsClient
    {
        private readonly IndicoStrawberryShakeClient _strawberryShake;
        private readonly IndicoClient _indicoClient;

        public ReviewsClient(IndicoClient indicoClient)
        {
            _indicoClient = indicoClient;
            _strawberryShake = indicoClient.IndicoStrawberryShakeClient;
        }

        public async Task<string> SubmitReviewAsync(int submissionId, JObject changes, bool rejected = false, bool? forceComplete = null, CancellationToken cancellationToken = default)
        {
            _indicoClient.Logger.LogDebug($"IndicoV2.Reviews.ReviewsClient.SubmitReviewAsync(): submitting v1 auto review for submission {submissionId}");
            var result = await _strawberryShake.Reviews().SubmitReview(submissionId, changes.ToString(), rejected, forceComplete, cancellationToken);
            _indicoClient.Logger.LogDebug($"IndicoV2.Reviews.ReviewsClient.SubmitReviewAsync(): submitted v1 auto review for submission {submissionId}");
            return result;
        }

        public async Task<string> SubmitReviewAsync(int submissionId, JArray changes, bool rejected = false, bool? forceComplete = null, CancellationToken cancellationToken = default)
        {
            _indicoClient.Logger.LogDebug($"IndicoV2.Reviews.ReviewsClient.SubmitReviewAsync(): submitting v3 auto review for submission {submissionId}");
            var result = await _strawberryShake.Reviews().SubmitReview(submissionId, changes.ToString(), rejected, forceComplete, cancellationToken);
            _indicoClient.Logger.LogDebug($"IndicoV2.Reviews.ReviewsClient.SubmitReviewAsync(): submitted v3 auto review for submission {submissionId}");
            return result;
        }

        public async Task<IEnumerable<ReviewDetailed>> GetReviewsAsync(int submissionId, CancellationToken cancellationToken = default)
        {
            var response = await _strawberryShake.Reviews().GetReviews(submissionId, cancellationToken);
            var result = response.Select(review => new ReviewDetailed()
            {
                Id = review.Id,
                SubmissionId = review.SubmissionId,
                CreatedAt = review.CreatedAt,
                CreatedBy = review.CreatedBy,
                StartedAt = review.StartedAt,
                CompletedAt = review.CompletedAt,
                Rejected = review.Rejected,
                ReviewType = (Submissions.Models.ReviewType?)review.ReviewType,
                Notes = review.Notes,
                Changes = JObject.Parse(review.Changes)

            });
            return result;
        }

    }
}