using System;
using Indico;
using Indico.Query;
using Indico.Entity;

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
            Model model = trainingModelWithProgress.Id(model_group_id).Query();
            Console.WriteLine(modelGroup.Name);
            Console.WriteLine($"training status : {1}", model.Status);
            Console.WriteLine($"percent complete : {1}", model.TrainingProgress.PercentComplete);
        }
    }
}