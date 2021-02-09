using IndicoV2.Reviews;
using IndicoV2.V1Adapters.Reviews;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        /// <summary>
        /// Gets <seealso cref="IReviewsClient"/>
        /// </summary>
        public static IReviewsClient Reviews(this IndicoClient indicoClient) =>
            new ReviewsV1ClientAdapter(indicoClient.LegacyClient);
    }
}
