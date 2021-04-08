using IndicoV2.DataSets;
using IndicoV2.V1Adapters.DataSets;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        /// <summary>
        /// Gets <seealso cref="IDataSetClient"/>
        /// </summary>
        /// <param name="indicoClient">Instance of <seealso cref="IndicoClient"/></param>
        /// <returns>Instance of <seealso cref="IDataSetClient"/></returns>
        public static IDataSetClient DataSets(this IndicoClient indicoClient) =>
            new DataSetClient(new DataSetsV1ClientAdapter(indicoClient.LegacyClient),
                indicoClient.IndicoStrawberryShakeClient.DataSets(), indicoClient.Storage());
    }
}
