using System;
using Indico;
using Indico.Query;
using Indico.Entity;
using Newtonsoft.Json.Linq;


namespace Examples
{
    class GetTrainingModelProgress
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a model group id");
                Environment.Exit(0);
            }

            int model_group_id = Int32.Parse(args[0]);

            IndicoConfig config = new IndicoConfig(
                host: "app.indico.io"             
            );

            IndicoClient client = new IndicoClient(config);
            ModelGroupQuery modelGroupQuery = client.ModelGroupQuery();
            TrainingModelWithProgressQuery trainingModelWithProgress = client.TrainingModelWithProgressQuery();
            ModelGroup modelGroup = modelGroupQuery.Id(model_group_id).Query();
            JObject trainingStatus = trainingModelWithProgress.Id(model_group_id).Query();
            Console.WriteLine(modelGroup.Name);
            Console.WriteLine(trainingStatus);
        }
    }
}