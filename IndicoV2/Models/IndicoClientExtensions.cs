using IndicoV2.Jobs;
using IndicoV2.Models;
using IndicoV2.V1Adapters.Models;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        /// <summary>
        /// Gets <seealso cref="IJobsClient"/>
        /// </summary>
        /// <param name="indicoClient">Indico client</param>
        /// <returns>Instance of <seealso cref="IJobsClient"/> /></returns>
        public static IModelClient Models(this IndicoClient indicoClient) => new V1ModelClientAdapter(indicoClient.LegacyClient);
    }
}
