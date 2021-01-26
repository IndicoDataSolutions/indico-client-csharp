using IndicoV2.Jobs;
using IndicoV2.V1Adapters.Jobs;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        public static IJobsClient Jobs(this IndicoClient indicoClient) => new V1JobsClientAdapter(indicoClient.LegacyClient);
    }
}
