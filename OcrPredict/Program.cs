using System;
using System.IO;
using System.Collections.Generic;
using Indico;
using Indico.Mutation;
using Indico.Jobs;
using Indico.Entity;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Indico.Storage;

namespace OcrPredict
{
    /// <summary>
    /// NOTE: This example is obsolete and will be rewritten.
    /// </summary>
    internal class Program
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
        private static async Task Main(string[] args)
        {
            // Replace this with your Model Group ID
            int mgId = 4352;

            var targetFiles = GetTargetFiles(args[0]);
            if (targetFiles.Count == 0)
            {
                Console.WriteLine("No files to process");
                Environment.Exit(0);
            }

            var client = new IndicoClient();

            var extractConfig = new JObject()
            {
                { "preset_config", "legacy" }
            };

            var texts = new List<string>();
            var ocrQuery = client.DocumentExtraction(extractConfig);

            foreach (string path in targetFiles)
            {
                Console.WriteLine(path);
                var ocrJob = await ocrQuery.Exec(path);

                var result = await ocrJob.Result();
                string resUrl = (string)result.GetValue("url");
                var blob = await client.RetrieveBlob(resUrl).Exec();
                var obj = blob.AsJSONObject();
                texts.Add((string)obj.GetValue("text"));
            }

            var mg = await client.ModelGroupQuery(mgId).Exec();
            
            string status = await client.ModelGroupLoad(mg).Exec();
            Console.WriteLine($"Model status = {status}");

            var job = await client.ModelGroupPredict(mg).Data(texts).Exec();

            var jobResult = await job.Results();
            Console.WriteLine(jobResult);
        }

        private static List<string> GetTargetFiles(string srcDir)
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
