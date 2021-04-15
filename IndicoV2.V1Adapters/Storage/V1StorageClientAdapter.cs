using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

        private Uri UploadUri => new Uri(new Uri(_clientLegacy.Config.GetAppBaseUrl()), "/storage/files/store");

        public V1StorageClientAdapter(IndicoClient clientLegacy) => _clientLegacy = clientLegacy;

        public async Task<Stream> GetAsync(Uri uri, CancellationToken cancellationToken)
        {
            var blob = await new RetrieveBlob(_clientLegacy) { Url = uri.ToString() }.Exec();
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

        public async Task<(string Name, string Meta)> UploadAsync(Stream content,
            string filePath,
            CancellationToken cancellationToken) =>
            (await UploadAsync(new[] {(filePath, content)}, cancellationToken)).SingleOrDefault();

        public async Task<(string Name, string Meta)[]> UploadAsync(IEnumerable<(string Path, Stream Content)> files,
            CancellationToken cancellationToken)
        {
            var content = await CreateRequest(files, cancellationToken);
            var response = await _clientLegacy.HttpClient.PostAsync(UploadUri, content, cancellationToken);

            using (var reader = new JsonTextReader(new StreamReader(await response.Content.ReadAsStreamAsync())))
            {
                var uploadResult = await JArray.LoadAsync(reader, cancellationToken);

                return uploadResult
                    .Select(t => (
                        Name: t.Value<string>("name"),
                        Meta: t.ToString()))
                    .ToArray();
            }
        }

        private async Task<HttpContent> CreateRequest(IEnumerable<(string Path, Stream Content)> files, CancellationToken cancellationToken)
        {
            Stream formDataStream = new MemoryStream();
            var encoding = Encoding.UTF8;
            var needsClRf = false;
            var boundary = $"----------{Guid.NewGuid():N}";
            var contentType = "multipart/form-data; boundary=" + boundary;

            foreach (var (path, content) in files)
            {
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsClRf)
                {
                    await formDataStream.WriteAsync(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"), cancellationToken);
                }

                needsClRf = true;

                var fileName = Path.GetFileName(path);
                var mimeType = MimeKit.MimeTypes.GetMimeType(fileName);
                var header =
                    $"--{boundary}\r\nContent-Disposition: form-data; name=\"{path}\"; filename=\"{fileName}\"\r\nContent-Type: {mimeType}\r\n\r\n";

                await formDataStream.WriteAsync(encoding.GetBytes(header), 0, encoding.GetByteCount(header), cancellationToken);
                await content.CopyToAsync(formDataStream, 81920, cancellationToken);
            }

            // Add the end of the request.  Start with a newline
            var footer = "\r\n--" + boundary + "--\r\n";
            await formDataStream.WriteAsync(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer), cancellationToken);

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            var formData = new byte[formDataStream.Length];
            await formDataStream.ReadAsync(formData, 0, formData.Length, cancellationToken);
            formDataStream.Close();

            // Add Content Headers
            HttpContent httpContent = new ByteArrayContent(formData);
            httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            httpContent.Headers.ContentLength = formData.Length;

            return httpContent;
        }
    }
}
