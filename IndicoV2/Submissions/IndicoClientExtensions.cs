using IndicoV2.Abstractions.Submissions;
using IndicoV2.V1Adapters.Submissions;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        public static ISubmissionsClient Submissions(this IndicoClient client) => new SubmissionsV1ClientAdapter(client.LegacyClient);
    }
}
