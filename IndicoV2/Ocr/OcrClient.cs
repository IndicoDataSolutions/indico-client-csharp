using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using IndicoV2.StrawberryShake;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IndicoV2.Storage;
using IndicoV2.Ocr.Models;

namespace IndicoV2.Ocr
{
    public class OcrClient : IOcrClient
    {
        private readonly IndicoStrawberryShakeClient _strawberryShake;
        private readonly IndicoClient _indicoClient;

        public OcrClient(IndicoClient indicoClient)
        {
            _indicoClient = indicoClient;
            _strawberryShake = indicoClient.IndicoStrawberryShakeClient;
        }

        internal async Task<JArray> Upload(List<string> filePaths)
        {
            var uploadRequest = new UploadFile(_indicoClient) { Files = filePaths };
            var arr = await uploadRequest.Call();
            return arr;
        }

        public async Task<string> ExtractDocumentAsync(string filePath, DocumentExtractionPreset preset, CancellationToken cancellationToken)
        {
            JArray fileMetadata;
            var files = new List<FileInput>();
            fileMetadata = await Upload(new List<string> { filePath });
            foreach (JObject uploadMeta in fileMetadata)
            {
                var meta = new JObject
                {
                    { "name", (string)uploadMeta.GetValue("name") },
                    { "path", (string)uploadMeta.GetValue("path") },
                    { "upload_type", (string)uploadMeta.GetValue("upload_type") }
                };

                var file = new FileInput
                {
                    Filename = (string)uploadMeta.GetValue("name"),
                    Filemeta = meta.ToString()
                };

                files.Add(file);
            }
            var config = new JObject
            {
                {"preset_config", preset.ToString("F").ToLower()}
            };

            var job = await _strawberryShake.Ocr().DocumentExtraction(files, config, cancellationToken);

            return job.DocumentExtraction.JobIds.First();
        }
        public async Task<string> GetExtractionResultAsync(Uri documentUri, CancellationToken cancellationToken) =>
            (await GetExtractionResultAsync<JToken>(documentUri, cancellationToken)).Value<string>("text");

        public async Task<TResult> GetExtractionResultAsync<TResult>(Uri documentUri, CancellationToken cancellationToken)
        {
            using (var docStream = await _indicoClient.Storage().GetAsync(documentUri, cancellationToken))
            using (var reader = new JsonTextReader(new StreamReader(docStream)))
            {
                return JsonSerializer.Create().Deserialize<TResult>(reader);
            }
        }
    }
}