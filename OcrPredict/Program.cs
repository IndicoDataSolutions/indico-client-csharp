using System;
using System.IO;
using System.Collections.Generic;
using Indico;
using Indico.Mutation;
using Indico.Jobs;
using Indico.Entity;
using Newtonsoft.Json.Linq;

namespace OcrPredict
{
    class Program
    {
        /* 
         * To actually run this example, you'll need to train a sequence
         * model on the Indico IPA Platform with the labeled-swaps-200.csv
         * file contained with this repo. Be aware that training will likely
         * take a couple hours. Once training is complete, you can run the
         * example by passing in the directory of the two sample PDF files in
         * the repo (Confirmation letter and Confirmation of Interest Rate Swap).
         * 
         * Before running, replace the Model Group ID (mgId) with the
         * ID for your trained model. You can find it on the model's Review page.
         */
        static void Main(string[] args)
        {
            // Replace this with your Model Group ID
            int mgId = 4352;

            List<string> targetFiles = GetTargetFiles(args[0]);
            if (targetFiles.Count == 0)
            {
                Console.WriteLine("No files to process");
                Environment.Exit(0);
            }

            IndicoClient client = new IndicoClient();

            JObject extractConfig = new JObject()
            {
                { "preset_config", "legacy" }
            };

            List<string> texts = new List<string>();
            DocumentExtraction ocrQuery = client.DocumentExtraction(extractConfig);

            foreach (string path in targetFiles)
            {
                Console.WriteLine(path);
                Job ocrJob = ocrQuery.Exec(path);

                string resUrl = (string)ocrJob.Result().GetValue("url");
                JObject obj = client.RetrieveBlob(resUrl).Exec().AsJSONObject();
                texts.Add((string)obj.GetValue("text"));
            }

            ModelGroup mg = client.ModelGroupQuery(mgId).Exec();
            
            String status = client.ModelGroupLoad(mg).Exec();
            Console.WriteLine($"Model status = {status}");

            Job job = client.ModelGroupPredict(mg).Data(texts).Exec();

            JArray jobResult = job.Results();
            Console.WriteLine(jobResult);
        }

        static List<string> GetTargetFiles(string srcDir)
        {
            var targetPaths = new List<string>();
            string[] ocrExts = { "*.pdf", "*.tif", "*.tiff" };

            if (Directory.Exists(srcDir) == true)
            {
                foreach (string fileExt in ocrExts)
                {
                    foreach (string srcPath in Directory.EnumerateFiles(srcDir, fileExt))
                    {
                        targetPaths.Add(srcPath);
                    }
                }
            }

            return (targetPaths);
        }
    }
}
