using IndicoV2.DataSets;
using IndicoV2.V1Adapters.DataSets;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        /// <summary>
        /// Gets <seealso cref="IDataSetClient"/>
        /// </summary>
        public static IDataSetClient DataSets(this IndicoClient indicoClient) =>
            new DataSetsV1ClientAdapter(indicoClient.LegacyClient);
    }
}
