using System;
using System.Collections.Generic;
using Indico;
using Indico.Mutation;
using Indico.Jobs;
using Indico.Storage;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Examples
{
    class SingleDocExtraction
    {
        /*
         * Run with your own PDF or use the sample Amtrak-Financials file
         * provided in this repo.
         */
        async static Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the filepath of a PDF to OCR");
                Environment.Exit(0);
            }

            IndicoConfig config = new IndicoConfig(
                host: "app.indico.io"
            );
            IndicoClient client = new IndicoClient(config);

            JObject extractConfig = new JObject()
            {
                { "preset_config", "standard" }
            };

            DocumentExtraction ocrQuery = client.DocumentExtraction(extractConfig);
            Job job = await ocrQuery.Exec(args[0]);

            JObject result = await job.Result();
            string url = (string) result.GetValue("url");
            Blob blob = await client.RetrieveBlob(url).Exec();
            Console.WriteLine(blob.AsJSONObject());
        }
    }
}