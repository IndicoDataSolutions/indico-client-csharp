using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Indico;
using Indico.Mutation;
using IndicoV2.Ocr;
using IndicoV2.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IndicoV2.V1Adapters.Ocr
{
    public class OcrV1ClientAdapter : IOcrClient
    {
        private readonly IndicoClient _indicoClientLegacy;
        private readonly IStorageClient _storage;

        public OcrV1ClientAdapter(IndicoClient indicoClientLegacy, IStorageClient storage)
        {
            _indicoClientLegacy = indicoClientLegacy;
            _storage = storage;
        }

        public async Task<string> ExtractDocumentAsync(string filePath, string configType, CancellationToken cancellationToken)
        {
            var config = new JObject
            {
                {"preset_config", configType}
            };
            var docExtraction =
                new DocumentExtraction(_indicoClientLegacy)
                {
                    JsonConfig = config,
                    //Files = new List<string> {filePath},
                };
            var job = await docExtraction.Exec(filePath, cancellationToken);

            return job.Id;
        }

        public async Task<string> GetExtractionResultAsync(Uri documentUri)
        {
            using (var docStream = await _storage.GetAsync(documentUri))
            using (var reader = new JsonTextReader(new StreamReader(docStream)))
            {
                var docJson = await JToken.ReadFromAsync(reader);

                return docJson.Value<string>("text");
            }
        }
    }
}