using IndicoV2.Submissions;
using IndicoV2.V1Adapters.Submissions;

// ReSharper disable once CheckNamespace
namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        /// <summary>
        /// Gets <seealso cref="ISubmissionsClient"/>
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static ISubmissionsClient Submissions(this IndicoClient client) => new SubmissionsV1ClientAdapter(client.LegacyClient);
    }
}
