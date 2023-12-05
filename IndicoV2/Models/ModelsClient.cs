using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake;
using IndicoV2.Models.Models;
using System.Linq;

namespace IndicoV2.Models
{
    public class ModelsClient : IModelClient
    {

        private readonly IndicoStrawberryShakeClient _strawberryShake;
        private readonly IndicoClient _indicoClient;

        public ModelsClient(IndicoClient indicoClient)
        {
            _indicoClient = indicoClient;
            _strawberryShake = indicoClient.IndicoStrawberryShakeClient;
        }


        public async Task<IModelGroup> GetGroup(int modelGroupId, CancellationToken cancellationToken)
        {
            var result = await _strawberryShake.Models().GetGroup(modelGroupId, cancellationToken);
            return ToModelGroup(result.ModelGroups.ModelGroups.FirstOrDefault());
        }

        [Obsolete("Models are now automatically loaded by IPA")]

        public async Task<string> LoadModel(int modelId, CancellationToken cancellationToken)
        {
            var result = await _strawberryShake.Models().LoadModel(modelId, cancellationToken);
            return result.ModelLoad.Status;
        }


        public async Task<string> Predict(int modelId, List<string> data, CancellationToken cancellationToken)
        {
            var result = await _strawberryShake.Models().Predict(modelId, data, cancellationToken);
            return result.ModelPredict.JobId;
        }

        public async Task<IEnumerable<IModel>> TrainingModelWithProgress(int modelGroupId, CancellationToken cancellationToken)
        {
            var result = await _strawberryShake.Models().TrainingModelWithProgress(modelGroupId, cancellationToken);
            return result.ModelGroups.ModelGroups.FirstOrDefault().Models.Select(x => ToModel(x)).ToList();
        }

        private Model ToModel(dynamic model) => new Model
        {
            Id = model.Id ?? 0,
            Status = model.Status.ToString(),
            TrainingProgressPercents = model.GetType().GetProperty("TrainingProgress") != null ? (float)model.TrainingProgress?.PercentComplete : 0f,
        };

        private ModelGroup ToModelGroup(IModelGroupQuery_ModelGroups_ModelGroups modelGroup) => new ModelGroup
        {
            Id = modelGroup.Id ?? 0,
            Name = modelGroup.Name,
            Status = modelGroup.Status.ToString(),
            SelectedModel = ToModel(modelGroup.SelectedModel)
        };
    }
}
