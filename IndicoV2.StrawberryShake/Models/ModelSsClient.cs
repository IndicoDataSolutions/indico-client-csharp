using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace IndicoV2.StrawberryShake.Models
{
    public class ModelSsClient : ErrorHandlingWrapper
    {
        private readonly ServiceProvider _services;

        public ModelSsClient(ServiceProvider services) => _services = services;

        public Task<IModelGroupProgressQueryResult> TrainingModelWithProgress(int modelGroupId, CancellationToken cancellationToken) =>
            ExecuteAsync(async () => await _services
            .GetRequiredService<ModelGroupProgressQueryQuery>().ExecuteAsync(modelGroupId, cancellationToken));
    }
}
