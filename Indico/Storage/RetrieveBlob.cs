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
        /// Url of Blob to retrieve
        /// </summary>
        public string Url
        {
            get => _url;
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

        /// <summary>
        /// Set the Blob Url and return this to call in a chain.
        /// </summary>
        /// <returns>RetrieveBlob</returns>
        /// <param name="url">Bolb Url</param>
        public RetrieveBlob SetUrl(string url)
        {
            this.Url = url;
            return this;
        }

        async Task<HttpResponseMessage> Retrieve()
        {
            HttpResponseMessage response = await this._client.HttpClient.GetAsync(this._url);
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
            if (this._url.Contains(".gz"))
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
        public Blob Exec()
        {
            return new Blob(this.GetStream().Result);
        }
    }
}
