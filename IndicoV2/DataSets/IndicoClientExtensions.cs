using IndicoV2.Reviews;
using IndicoV2.V1Adapters.Reviews;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        /// <summary>
        /// Gets <seealso cref="IReviewsClient"/>
        /// </summary>
        /// <param name="indicoClient">Instance of <seealso cref="IndicoClient"/></param>
        /// <returns>Instance of <seealso cref="IReviewsClient"/></returns>
        public static IReviewsClient Reviews(this IndicoClient indicoClient) =>
            new ReviewsV1ClientAdapter(indicoClient.LegacyClient);
    }
}
