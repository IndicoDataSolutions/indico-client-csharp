using IndicoV2.Jobs;
using IndicoV2.V1Adapters.Jobs;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        /// <summary>
        /// Gets <seealso cref="IJobsClient"/>
        /// </summary>
        /// <param name="indicoClient">Indico client</param>
        /// <returns>Instance of <seealso cref="IJobsClient"/> /></returns>
        public static IJobsClient Jobs(this IndicoClient indicoClient) => new V1JobsClientAdapter(indicoClient.LegacyClient, new JobStatusConverter());
    }
}
