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

        public Stream AsStream()
        {
            return this.data;
        }

        public string AsString()
        {
            StreamReader reader = new StreamReader(this.data);
            return reader.ReadToEndAsync().Result;
        }

        public JObject AsJSONObject()
        {
            string jsonString = this.AsString();
            return JObject.Parse(jsonString);
        }

        public JArray AsJSONArray()
        {
            string jsonString = this.AsString();
            return JArray.Parse(jsonString);
        }
    }
}
