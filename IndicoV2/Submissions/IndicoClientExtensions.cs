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
        /// <param name="indicoClient">Instance of <seealso cref="IndicoClient"/></param>
        /// <returns>Instance of <seealso cref="ISubmissionsClient"/></returns>
        public static ISubmissionsClient Submissions(this IndicoClient indicoClient) => new SubmissionsV1ClientAdapter(indicoClient.LegacyClient);
    }
}
