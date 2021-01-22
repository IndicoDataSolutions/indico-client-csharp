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
    internal class SingleDocExtraction
    {
        /*
         * Run with your own PDF or use the sample Amtrak-Financials file
         * provided in this repo.
         */
        private static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the filepath of a PDF to OCR");
                Environment.Exit(0);
            }

            var config = new IndicoConfig(
                host: "app.indico.io"
            );
            var client = new IndicoClient(config);

            var extractConfig = new JObject()
            {
                { "preset_config", "standard" }
            };

            var ocrQuery = client.DocumentExtraction(extractConfig);
            var job = await ocrQuery.Exec(args[0]);

            var result = await job.Result();
            string url = (string) result.GetValue("url");
            var blob = await client.RetrieveBlob(url).Exec();
            Console.WriteLine(blob.AsJSONObject());
        }
    }
}