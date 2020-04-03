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
      IndicoConfig config = new IndicoConfig(
          host: "dev.indico.io",
          tokenPath: "__TOKEN_DIRECTORY__"
      );
      IndicoClient client = new IndicoClient(config);
      ModelGroupQuery modelGroupQuery = client.ModelGroupQuery();
      TrainingModelWithProgressQuery trainingModelWithProgress = client.TrainingModelWithProgressQuery();
      ModelGroup modelGroup = modelGroupQuery.Id(__MODEL_ID__).Query();
      Model model = trainingModelWithProgress.Id(__MODEL_ID__).Query();
      Console.WriteLine(modelGroup.Name);
      Console.WriteLine($"training status : {1}", model.Status);
      Console.WriteLine($"percent complete : {1}", model.TrainingProgress.PercentComplete);
    }
  }
}
