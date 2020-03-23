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

        public RetrieveBlob(IndicoClient client)
        {
            this._client = client;
        }

        public async Task<string> GZipDecompress(Stream compressed)
        {
            string uncompressed;
            GZipStream gis = new GZipStream(compressed, CompressionMode.Decompress);
            StreamReader reader = new StreamReader(gis);
            uncompressed = await reader.ReadToEndAsync();
            return uncompressed;
        }

        public RetrieveBlob Url(string url)
        {
            url = url.Replace("\"", "");
            // Drop gzip
            string path = new Uri(url).PathAndQuery;
            this._url = this._client.Config.GetAppBaseUrl() + path;
            Console.WriteLine(this._url);
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

        public Blob Execute()
        {
            return new Blob(this.GetStream().Result);
        }
    }
}
