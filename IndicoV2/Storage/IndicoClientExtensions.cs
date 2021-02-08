using IndicoV2.Storage;
using IndicoV2.V1Adapters.Storage;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        public static IStorageClient Storage(this IndicoClient client) => new V1StorageClientAdapter(client.LegacyClient);
    }
}
