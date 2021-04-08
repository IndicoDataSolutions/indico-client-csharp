using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Indico.Storage;
using IndicoV2.Storage;
using IndicoV2.Storage.Models;
using IndicoV2.V1Adapters.Storage.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using IndicoClient = Indico.IndicoClient;

namespace IndicoV2.V1Adapters.Storage
{
    public class V1StorageClientAdapter : IStorageClient
    {
        private readonly IndicoClient _clientLegacy;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy(),
            },
        };

        public V1StorageClientAdapter(IndicoClient clientLegacy) => _clientLegacy = clientLegacy;

        public async Task<Stream> GetAsync(Uri uri, CancellationToken cancellationToken)
        {
            var blob = await new RetrieveBlob(_clientLegacy) {Url = uri.ToString()}.Exec();
            var result = blob.AsStream();

            return result;
        }

        public async Task<IEnumerable<IFileMetadata>> UploadAsync(IEnumerable<string> filePaths, CancellationToken cancellationToken)
        {
            var metadata = await new UploadFile(_clientLegacy) {Files = filePaths.ToList()}.Call(cancellationToken);

            return DeserializeMetadata(metadata);
        }

        public IEnumerable<IFileMetadata> DeserializeMetadata(JArray metadata)
        {
            var serializer = JsonSerializer.Create(_jsonSerializerSettings);
            var filesMetadata = metadata.Select(item => item.ToObject<FileMetadata>(serializer));

            return filesMetadata;
        }

        public JArray Serialize(IEnumerable<IFileMetadata> filesMetadata)
        {
            var serializer = JsonSerializer.Create(_jsonSerializerSettings);
            var serializedMetadata = JArray.FromObject(filesMetadata, serializer);

            return serializedMetadata;
        }
    }
}
