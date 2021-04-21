using IndicoV2.Extensions.DataSets;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        public static IDataSetAwaiter DataSetAwaiter(this IndicoClient indicoClient) =>
            new DataSetAwaiter(indicoClient.DataSets());
    }
}
