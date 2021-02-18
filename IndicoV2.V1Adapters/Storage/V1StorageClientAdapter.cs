using System;
using System.IO;
using System.Threading.Tasks;
using Indico.Storage;
using IndicoV2.Storage;
using IndicoClient = Indico.IndicoClient;

namespace IndicoV2.V1Adapters.Storage
{
    public class V1StorageClientAdapter : IStorageClient
    {
        private readonly IndicoClient _clientLegacy;

        public V1StorageClientAdapter(IndicoClient clientLegacy) => _clientLegacy = clientLegacy;

        public async Task<Stream> GetAsync(Uri uri)
        {
            var blob = await new RetrieveBlob(_clientLegacy) {Url = uri.ToString()}.Exec();
            var result = blob.AsStream();

            return result;
        }
    }
}
