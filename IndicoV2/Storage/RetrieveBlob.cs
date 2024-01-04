using System;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Net.Http;
using System.IO;

namespace IndicoV2.Storage
{
    public class RetrieveBlob
    {
        private readonly IndicoClient _client;
        private string _url;

        /// <summary>
        /// Get/Set the Blob Storage URL
        /// </summary>
        public string Url
        {
            get => _url;
            set {
                string url = value.Replace("\"", "");
                // Drop gzip
                string path = new Uri(url).PathAndQuery;
                _url = _client.BaseUri + path;
            }
        }

        /// <summary>
        /// RetrieveBlob Constructor
        /// </summary>
        /// <param name="client"></param>
        public RetrieveBlob(IndicoClient client) => _client = client;

        /// <summary>
        /// Decompresses Gzip Stream
        /// </summary>
        /// <returns>Async String</returns>
        /// <param name="compressed">Compressed Stream</param>
        public async Task<string> GZipDecompress(Stream compressed)
        {
            string uncompressed;
            var gis = new GZipStream(compressed, CompressionMode.Decompress);
            var reader = new StreamReader(gis);
            uncompressed = await reader.ReadToEndAsync();
            return uncompressed;
        }

        private async Task<HttpResponseMessage> Retrieve()
        {
            var response = await _client.HttpClient.GetAsync(Url);
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            else
            {
                throw new IOException("Failed to retrieve blob at url " + _url);
            }
        }

        /// <summary>
        /// Retrieve the blob and decompress if needed
        /// </summary>
        /// <returns>Stream</returns>
        private async Task<Stream> GetStream()
        {
            var httpResponseMessage = await Retrieve();
            var data = await httpResponseMessage.Content.ReadAsStreamAsync();
            if (Url.Contains(".gz"))
            {
                return new GZipStream(data, CompressionMode.Decompress);
            }
            else
            {
                return data;
            }
        }

        /// <summary>
        /// Retrieves Blob
        /// </summary>
        /// <returns>Blob</returns>
        public async Task<Blob> Exec()
        {
            var stream = await GetStream();
            return new Blob(stream);
        }
    }
}
