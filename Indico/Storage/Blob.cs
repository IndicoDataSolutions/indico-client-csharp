using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Indico.Storage
{
    public class Blob
    {
        Stream _data = null;

        /// <summary>
        /// Blob constructor
        /// </summary>
        /// <param name="data"></param>
        public Blob(Stream data)
        {
            this._data = data;
        }

        public Blob(HttpResponseMessage response)
        {
            this._data = response.Content.ReadAsStreamAsync().Result;
        }

        /// <summary>
        /// Returns Blob as Stream
        /// </summary>
        /// <returns>Stream</returns>
        public Stream AsStream()
        {
            return this._data;
        }

        /// <summary>
        /// Returns Blob as string
        /// </summary>
        /// <returns>string</returns>
        public string AsString()
        {
            StreamReader reader = new StreamReader(this._data);
            return reader.ReadToEndAsync().Result;
        }

        /// <summary>
        /// Returns Blob as JSONObject
        /// </summary>
        /// <returns>JObject</returns>
        public JObject AsJSONObject()
        {
            string jsonString = this.AsString();
            return JObject.Parse(jsonString);
        }

        /// <summary>
        /// Returns Blob as JSONArray
        /// </summary>
        /// <returns>JArray</returns>
        public JArray AsJSONArray()
        {
            string jsonString = this.AsString();
            return JArray.Parse(jsonString);
        }
    }
}
