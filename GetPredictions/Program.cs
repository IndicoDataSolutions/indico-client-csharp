﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Indico;
using Indico.Jobs;
using Indico.Entity;
using Newtonsoft.Json.Linq;

namespace GetPredictions
{
    class Program
    {
        static void Main(string[] args)
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

            IndicoConfig config = new IndicoConfig(
                host: "app.indico.io"
            );

            IndicoClient client = new IndicoClient(config);

            ModelGroup mg = client.ModelGroupQuery(mgId).Exec();

            // Load Model
            String status = client.ModelGroupLoad(mg).Exec();

            List<string> reviews = new List<string>()
            {
                "This was the best food of our trip. Fantastic experience!",
                "The service was rude and the food was awful. Don\'t waste your time!"
            };

            Job job = client.ModelGroupPredict(mg).Data(reviews).Exec();

            JArray jobResult = job.Results();
            Console.WriteLine(jobResult);
        }
    }
}