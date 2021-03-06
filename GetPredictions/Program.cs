﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Indico;
using Indico.Jobs;
using Indico.Entity;
using Newtonsoft.Json.Linq;

namespace GetPredictions
{
    internal class Program
    {
        /*
         * To actually run this example, you need to train a classifier in
         * the Indico IPA Platform using the reviews.csv file included in
         * this repo. After the model is trained, just pass its model group
         * ID into this sample as a command line arg. You can find the model
         * group ID on your trained model's Review page.
         */
        private static async Task Main(string[] args)
        {
            int mgId = -1;

            if (args.Length != 1)
            {
                Console.WriteLine("Please provide model group and selected model IDs");
                System.Environment.Exit(0);
            }
            else
            {
                mgId = Int32.Parse(args[0]);
            }

            var config = new IndicoConfig(
                host: "app.indico.io"
            );

            var client = new IndicoClient(config);

            var mg = await client.ModelGroupQuery(mgId).Exec();

            // Load Model
            string status = await client.ModelGroupLoad(mg).Exec();

            var reviews = new List<string>()
            {
                "This was the best food of our trip. Fantastic experience!",
                "The service was rude and the food was awful. Don\'t waste your time!"
            };

            var job = await client.ModelGroupPredict(mg).Data(reviews).Exec();

            var jobResult = await job.Results();
            Console.WriteLine(jobResult);
        }
    }
}
