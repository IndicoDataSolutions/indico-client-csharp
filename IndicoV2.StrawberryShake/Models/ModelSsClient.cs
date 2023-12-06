using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace IndicoV2.StrawberryShake.Models
{
    public class ModelSsClient : ErrorHandlingWrapper
    {
        private readonly ServiceProvider _services;

        public ModelSsClient(ServiceProvider services) => _services = services;

        public async Task<IModelGroupQueryResult> GetGroup(int modelGroupId, CancellationToken cancellationToken)
        {
            var result = await ExecuteAsync(() => _services
            .GetRequiredService<ModelGroupQueryQuery>().ExecuteAsync(new List<int?> { modelGroupId }, cancellationToken));
            return result;
        }

        public Task<IModelLoadResult> LoadModel(int modelId, CancellationToken cancellationToken) =>
            ExecuteAsync(async () => await _services
            .GetRequiredService<ModelLoadMutation>().ExecuteAsync(modelId, cancellationToken));
        public async Task<IPredictModelResult> Predict(int modelId, List<string> data, CancellationToken cancellationToken)
        {
            var result = await ExecuteAsync(() => _services
            .GetRequiredService<PredictModelMutation>().ExecuteAsync(modelId, data, cancellationToken));
            return result;
        }

        public Task<IModelGroupProgressQueryResult> TrainingModelWithProgress(int modelGroupId, CancellationToken cancellationToken) =>
            ExecuteAsync(async () => await _services
            .GetRequiredService<ModelGroupProgressQueryQuery>().ExecuteAsync(modelGroupId, cancellationToken));
    }
}
