using IndicoV2.Storage;
using IndicoV2.V1Adapters.Storage;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        /// <summary>
        /// Gets <seealso cref="IStorageClient"/>
        /// </summary>
        /// <param name="indicoClient">Instance of <seealso cref="IndicoClient"/></param>
        /// <returns>Instance of <seealso cref="IStorageClient"/></returns>
        public static IStorageClient Storage(this IndicoClient indicoClient) => new V1StorageClientAdapter(indicoClient.LegacyClient);
    }
}
