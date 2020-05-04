using System;
using Indico;
using Indico.Query;
using Indico.Entity;
using Newtonsoft.Json.Linq;


namespace Examples
{
    class GetTrainingModelProgress
    {
        /*
         * Get the progress (% complete) of a training model group. This
         * is handy as some sequence models can take a very long time (hours)
         * to train. Pass your model's Model Group ID on the command line.
         * You can find the model group ID on your model's Review page.
         */
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a model group id");
                Environment.Exit(0);
            }

            int mgId = Int32.Parse(args[0]);

            IndicoConfig config = new IndicoConfig(
                host: "app.indico.io"             
            );

            IndicoClient client = new IndicoClient(config);
            ModelGroup mg = client.ModelGroupQuery(mgId).Exec();
            JArray trainingStatus = client.TrainingModelWithProgressQuery(mg).Exec();
            
            Console.WriteLine(mg.Name);
            Console.WriteLine(trainingStatus);                     
        }
    }
}