using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Indico;
using Indico.Mutation;
using IndicoV2.Ocr;
using IndicoV2.Ocr.Models;
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

        public async Task<string> ExtractDocumentAsync(string filePath, DocumentExtractionPreset preset, CancellationToken cancellationToken)
        {
            var config = new JObject
            {
                {"preset_config", preset.ToString("F").ToLower()}
            };
            var docExtraction =
                new DocumentExtraction(_indicoClientLegacy)
                {
                    JsonConfig = config,
                };
            var job = await docExtraction.Exec(filePath, cancellationToken);

            return job.Id;
        }

        public async Task<string> GetExtractionResultAsync(Uri documentUri, CancellationToken cancellationToken) =>
            (await GetExtractionResultAsync<JToken>(documentUri, cancellationToken)).Value<string>("text");

        public async Task<TResult> GetExtractionResultAsync<TResult>(Uri documentUri, CancellationToken cancellationToken)
        {
            using (var docStream = await _storage.GetAsync(documentUri, cancellationToken))
            using (var reader = new JsonTextReader(new StreamReader(docStream)))
            {
                return JsonSerializer.Create().Deserialize<TResult>(reader);
            }
        }
    }
}