using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Indico.Storage
{
    public class Blob
    {
        Stream data = null;

        public Blob(Stream data)
        {
            this.data = data;
        }

        public Blob(HttpResponseMessage response)
        {
            this.data = response.Content.ReadAsStreamAsync().Result;
        }

        /// <summary>
        /// Returns Blob as Stream
        /// </summary>
        /// <returns>Stream</returns>
        public Stream AsStream()
        {
            return this.data;
        }

        /// <summary>
        /// Returns Blob as string
        /// </summary>
        /// <returns>string</returns>
        public string AsString()
        {
            StreamReader reader = new StreamReader(this.data);
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
