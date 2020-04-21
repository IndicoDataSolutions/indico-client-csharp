using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Indico;
using Indico.Jobs;
using Indico.Entity;
using Indico.Mutation;
using Newtonsoft.Json.Linq;

namespace GetPredictions
{
    class Program
    {
        static void Main(string[] args)
        {
            int mg_id = -1;

            if (args.Length != 1)
            {
                Console.WriteLine("Please provide model group and selected model IDs");
                System.Environment.Exit(0);
            }
            else
            {
                mg_id = Int32.Parse(args[0]);
            }

            IndicoConfig config = new IndicoConfig(
                host: "app.indico.io"
            );

            IndicoClient client = new IndicoClient(config);

            ModelGroup mg = client.ModelGroupQuery()
                                  .Id(mg_id)
                                  .Query();

            // Load Model
            String status = client.ModelGroupLoad()
                                  .ModelGroup(mg)
                                  .Execute();

            List<string> reviews = new List<string>()
            {
                "This was the best food of our trip. Fantastic experience!",
                "The service was rude and the food was awful. Don\'t waste your time!"
            };

            Job job = client.ModelGroupPredict()
                .ModelGroup(mg)
                .Data(reviews)
                .Execute();

            JArray jobResult = job.Results();
        }
    }
}
