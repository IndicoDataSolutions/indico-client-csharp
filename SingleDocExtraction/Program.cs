﻿using System;
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
                host: "app.indico.io"
            );
            IndicoClient client = new IndicoClient(config);

            JObject extractConfig = new JObject()
            {
                { "preset_config", "standard" }
            };

            DocumentExtraction ocr = client.DocumentExtraction(extractConfig);
            Job job = ocr.Exec(args[0]);
            
            string url = (string)job.Result().GetValue("url");
            Blob blob = client.RetrieveBlob(url).Exec();
            Console.WriteLine(blob.AsJSONObject());
        }
    }
}