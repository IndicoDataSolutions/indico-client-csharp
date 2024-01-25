using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using IndicoV2.StrawberryShake;
using Newtonsoft.Json.Linq;

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

        public async Task<string> SubmitReviewAsync(int submissionId, JObject changes, bool rejected = false, bool? forceComplete = null, CancellationToken cancellationToken = default) =>
            await _strawberryShake.Reviews().SubmitReview(submissionId, changes, rejected, forceComplete, cancellationToken);


    }
}