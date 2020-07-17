using System;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Net.Http;
using System.IO;

namespace Indico.Storage
{
    public class RetrieveBlob
    {
        IndicoClient _client;
        string _url;
        
        /// <summary>
        /// Get/Set the Blob Storage URL
        /// </summary>
        public string Url
        {
            get => this._url;
            set {
                string url = value.Replace("\"", "");
                // Drop gzip
                string path = new Uri(url).PathAndQuery;
                this._url = this._client.Config.GetAppBaseUrl() + path;
            }
        }

        /// <summary>
        /// RetrieveBlob Constructor
        /// </summary>
        /// <param name="client"></param>
        public RetrieveBlob(IndicoClient client)
        {
            this._client = client;
        }

        /// <summary>
        /// Decompresses Gzip Stream
        /// </summary>
        /// <returns>Async String</returns>
        /// <param name="compressed">Compressed Stream</param>
        public async Task<string> GZipDecompress(Stream compressed)
        {
            string uncompressed;
            GZipStream gis = new GZipStream(compressed, CompressionMode.Decompress);
            StreamReader reader = new StreamReader(gis);
            uncompressed = await reader.ReadToEndAsync();
            return uncompressed;
        }

        async Task<HttpResponseMessage> Retrieve()
        {
            HttpResponseMessage response = await this._client.HttpClient.GetAsync(this.Url);
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
        async Task<Stream> GetStream()
        {
            HttpResponseMessage httpResponseMessage = await this.Retrieve();
            Stream data = await httpResponseMessage.Content.ReadAsStreamAsync();
            if (this.Url.Contains(".gz"))
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
        async public Task<Blob> Exec()
        {
            var stream = await this.GetStream();
            return new Blob(stream);
        }
    }
}
