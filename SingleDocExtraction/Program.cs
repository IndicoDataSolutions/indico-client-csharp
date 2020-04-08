using System;
using System.Threading;
using System.Collections.Generic;
using Indico;
using Indico.Mutation;
using Indico.Jobs;
using Indico.Storage;
using Newtonsoft.Json.Linq;

namespace Examples
{
    class SingleDocExtraction
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the filepath of a PDF to OCR");
                Environment.Exit(0);
            }

            IndicoConfig config = new IndicoConfig(
                host: "app.indico.io",
                tokenPath: "c:\\Users\\dzere"
            );
            IndicoClient client = new IndicoClient(config);

            List<string> files = new List<string>()
            {
                args[0]
            };

            JObject json = new JObject()
            {
                { "preset_config", "simple" }
            };

            DocumentExtraction extraction = client.DocumentExtraction();
            List<Job> jobs = extraction.Files(files).JsonConfig(json).Execute();
            Job job = jobs[0];
            JObject obj = job.Result().Result;
            string url = (string)obj.GetValue("url");
            RetrieveBlob retrieveBlob = client.RetrieveBlob();
            Blob blob = retrieveBlob.Url(url).Execute();
            Console.WriteLine(blob.AsJSONObject());
        }
    }
}