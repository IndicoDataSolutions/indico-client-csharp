using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace IndicoV2.Storage
{
    public class Blob
    {
        private readonly Stream _data = null;

        /// <summary>
        /// Blob constructor
        /// </summary>
        /// <param name="data"></param>
        public Blob(Stream data) => _data = data;

        public Blob(HttpResponseMessage response) => _data = response.Content.ReadAsStreamAsync().Result;

        /// <summary>
        /// Returns Blob as Stream
        /// </summary>
        /// <returns>Stream</returns>
        public Stream AsStream() => _data;

        /// <summary>
        /// Returns Blob as string
        /// </summary>
        /// <returns>string</returns>
        public string AsString()
        {
            var reader = new StreamReader(_data);
            return reader.ReadToEndAsync().Result;
        }

        /// <summary>
        /// Returns Blob as JSONObject
        /// </summary>
        /// <returns>JObject</returns>
        public JObject AsJSONObject()
        {
            string jsonString = AsString();
            return JObject.Parse(jsonString);
        }

        /// <summary>
        /// Returns Blob as JSONArray
        /// </summary>
        /// <returns>JArray</returns>
        public JArray AsJSONArray()
        {
            string jsonString = AsString();
            return JArray.Parse(jsonString);
        }
    }
}
